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
            .Setup(useCase => useCase.ExecuteAsync(It.IsAny<CreateContactCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        var controller = CreateController(create: useCaseMock.Object);
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
            .Setup(useCase => useCase.ExecuteAsync(It.IsAny<CreateContactCommand>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException(expectedError));

        var controller = CreateController(create: useCaseMock.Object);
        var command = new CreateContactCommand("John", "Doe", new EmailAddress("john@doe.com"), "123", "Note");

        // Act
        var response = await controller.CreateContactAsync(command, CancellationToken.None);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(response);
        Assert.Equal(expectedError, badRequestResult.Value);
    }

    [Fact]
    public async Task Read_shouldReturnOk_WhenUseCaseSucceeds()
    {
        // Arrange
        var useCaseMock = new Mock<ReadContactUseCase>(MockBehavior.Default, default!);
        var expectedResult = new ReadContactResult(Guid.NewGuid(), "John", "Doe", new EmailAddress("john@doe.com"), "123", "Note");

        useCaseMock
            .Setup(useCase => useCase.ExecuteAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        var controller = CreateController(read: useCaseMock.Object);

        // Act
        var response = await controller.ReadContactAsync(expectedResult.Id, CancellationToken.None);

        // Assert
        var readResult = Assert.IsType<OkObjectResult>(response);

        var value = Assert.IsType<ContactDto>(readResult.Value);
        Assert.Equal(expectedResult.FirstName, value.FirstName);
    }

    [Fact]
    public async Task Read_shouldReturnNotFound_WhenContactDoesNotExist()
    {
        // Arrange
        var useCaseMock = new Mock<ReadContactUseCase>(MockBehavior.Default, default!);
        var expectedError = "Contact not found.";

        useCaseMock
            .Setup(useCase => useCase.ExecuteAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new KeyNotFoundException(expectedError));

        var controller = CreateController(read: useCaseMock.Object);

        // Act
        var response = await controller.ReadContactAsync(Guid.NewGuid(), CancellationToken.None);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(response);
        Assert.Equal(expectedError, notFoundResult.Value);
    }

    [Fact]
    public async Task Update_shouldReturnOk_WhenUseCaseSucceeds()
    {
        // Arrange
        var useCaseMock = new Mock<UpdateContactUseCase>(MockBehavior.Default, default!, default!);

        var contactId = Guid.NewGuid();

        var expectedResult = new ReadContactResult(contactId, "John", "Doe", new EmailAddress("john@doe.com"), "123", "Note");
        var command = new UpdateContactCommand(contactId, "John", "Doe", new EmailAddress("john@doe.com"), "123", "Note");

        useCaseMock
            .Setup(useCase => useCase.ExecuteAsync(It.IsAny<UpdateContactCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        var controller = CreateController(update: useCaseMock.Object);

        // Act
        var response = await controller.UpdateContactAsync(contactId, command, CancellationToken.None);

        // Assert
        var updateResult = Assert.IsType<OkObjectResult>(response);
        var value = Assert.IsType<ContactDto>(updateResult.Value);

        Assert.Equal(expectedResult.FirstName, value.FirstName);

        useCaseMock.Verify(useCase => useCase.ExecuteAsync(command, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Update_shouldReturnBadRequest_WhenRouteIdDoesNotMatchBodyId()
    {
        // Arrange
        var useCaseMock = new Mock<UpdateContactUseCase>(MockBehavior.Default, default!, default!);
        var controller = CreateController(update: useCaseMock.Object);

        var routeId = Guid.NewGuid();
        var differentBodyId = Guid.NewGuid();

        var command = new UpdateContactCommand(differentBodyId, "John", "Doe", new EmailAddress("john@doe.com"), "123", "Note");

        // Act
        var response = await controller.UpdateContactAsync(routeId, command, CancellationToken.None);

        // Assert
        var badRequest = Assert.IsType<BadRequestObjectResult>(response);
        Assert.Equal("Body id and route id do not match.", badRequest.Value);

        useCaseMock.Verify(x => x.ExecuteAsync(It.IsAny<UpdateContactCommand>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Update_shouldReturnNotFound_WhenContactDoesNotExist()
    {
        // Arrange
        var useCaseMock = new Mock<UpdateContactUseCase>(MockBehavior.Default, default!, default!);
        var expectedError = "Contact not found.";

        var contactId = Guid.NewGuid();

        useCaseMock
            .Setup(useCase => useCase.ExecuteAsync(It.IsAny<UpdateContactCommand>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new KeyNotFoundException(expectedError));

        var command = new UpdateContactCommand(contactId, "John", "Doe", new EmailAddress("john@doe.com"), "123", "Note");

        var controller = CreateController(update: useCaseMock.Object);

        // Act
        var response = await controller.UpdateContactAsync(contactId, command, CancellationToken.None);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(response);
        Assert.Equal(expectedError, notFoundResult.Value);
    }

    [Fact]
    public async Task Delete_shouldReturnNoContent_WhenUseCaseSucceeds()
    {
        // Arrange
        var useCaseMock = new Mock<DeleteContactUseCase>(MockBehavior.Default, default!, default!);
        var contactId = Guid.NewGuid();

        useCaseMock
            .Setup(useCase => useCase.ExecuteAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var controller = CreateController(delete: useCaseMock.Object);

        // Act
        var response = await controller.DeleteContact(contactId, CancellationToken.None);

        // Assert
        Assert.IsType<NoContentResult>(response);
        useCaseMock.Verify(useCase => useCase.ExecuteAsync(contactId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Delete_shouldReturnNotFound_WhenContactDoesNotExist()
    {
        // Arrange
        var useCaseMock = new Mock<DeleteContactUseCase>(MockBehavior.Default, default!, default!);
        var expectedError = "Contact not found.";

        useCaseMock
            .Setup(useCase => useCase.ExecuteAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new KeyNotFoundException(expectedError));

        var controller = CreateController(delete: useCaseMock.Object);

        // Act
        var response = await controller.DeleteContact(Guid.NewGuid(), CancellationToken.None);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(response);
        Assert.Equal(expectedError, notFoundResult.Value);

        useCaseMock.Verify(useCase => useCase.ExecuteAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    private ContactController CreateController(
        CreateContactUseCase? create = null,
        ReadContactUseCase? read = null,
        UpdateContactUseCase? update = null,
        DeleteContactUseCase? delete = null,
        ListContactsUseCase? list = null)
    {
        return new ContactController(
            create ?? new Mock<CreateContactUseCase>(MockBehavior.Default, default!, default!).Object,
            read ?? new Mock<ReadContactUseCase>(MockBehavior.Default, default!).Object,
            update ?? new Mock<UpdateContactUseCase>(MockBehavior.Default, default!, default!).Object,
            delete ?? new Mock<DeleteContactUseCase>(MockBehavior.Default, default!, default!).Object,
            list ?? new Mock<ListContactsUseCase>(MockBehavior.Default, default!).Object
        );
    }
    
    // TODO: Add tests for listing all contacts
}