using JetBrains.Annotations;
using Microsoft.AspNetCore.Mvc;
using MissSolitude.API.Controllers;
using MissSolitude.Application.Commands.Contact;
using MissSolitude.Application.Results.Contact;
using MissSolitude.Application.UseCases.Contact;
using MissSolitude.Contracts.Contact;
using MissSolitude.Domain.ValueObjects;
using Moq;

namespace MissSolitude.Tests.Controllers;

[TestSubject(typeof(ContactController))]
public class ContactControllerTest
{

    [Fact]
    public async Task Create_shouldReturnOk_WhenUseCaseSucceeds()
    {
        // Arrange
        var useCaseMock = new Mock<CreateContactUseCase>(MockBehavior.Default, default!, default!);
        var expectedResult = new CreateContactResult(Guid.NewGuid(), "John", "Doe", new EmailAddress("john@doe.com"), "123", "Note");
        
        useCaseMock
            .Setup(entity => entity.ExecuteAsync(It.IsAny<CreateContactCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);
        
        var controller = new ContactController(useCaseMock.Object, default!, default!, default!);
        var command = new CreateContactCommand("John", "Doe", new EmailAddress("john@doe.com"), "123", "Note");
        
        // Act
        var response = await controller.CreateContactAsync(command, CancellationToken.None);
        
        // Assert
        var createdResult = Assert.IsType<CreatedAtRouteResult>(response);
        
        var value = Assert.IsType<ContactDto>(createdResult.Value);
        Assert.Equal(expectedResult.FirstName, value.FirstName);
    }
    
    [Fact]
    public async Task Create_shouldReturnBadRequest_WhenContactAlreadyExists()
    {
        // Arrange
        var useCaseMock = new Mock<CreateContactUseCase>(MockBehavior.Default, default!, default!);
        var expectedError = "Contact already exists.";
        
        useCaseMock
            .Setup(entity => entity.ExecuteAsync(It.IsAny<CreateContactCommand>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException(expectedError));
        
        var controller = new ContactController(useCaseMock.Object, default!, default!, default!);
        var command = new CreateContactCommand("John", "Doe", new EmailAddress("john@doe.com"), "123", "Note");
        
        // Act
        var response = await controller.CreateContactAsync(command, CancellationToken.None);
        
        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(response);
        Assert.Equal(expectedError, badRequestResult.Value);
    }
}