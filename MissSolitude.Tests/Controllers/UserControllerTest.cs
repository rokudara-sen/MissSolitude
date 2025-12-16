using JetBrains.Annotations;
using Microsoft.AspNetCore.Mvc;
using MissSolitude.API.Controllers;
using MissSolitude.Application.Commands.User;
using MissSolitude.Application.Results.User;
using MissSolitude.Application.UseCases.User;
using MissSolitude.Contracts.User;
using MissSolitude.Domain.ValueObjects;
using Moq;

namespace MissSolitude.Tests.Controllers;

[TestSubject(typeof(UserController))]
public class UserControllerTest
{

    [Fact]
    public async Task Create_shouldReturnOk_WhenUseCaseSucceeds()
    {
        // Arrange
        var useCaseMock = new Mock<CreateUserUseCase>(MockBehavior.Default, default!, default!, default!);
        var expectedResult = new CreateUserResult(Guid.NewGuid(), "John", new EmailAddress("john@doe.com"));
        
        useCaseMock
            .Setup(useCase => useCase.ExecuteAsync(It.IsAny<CreateUserCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);
        
        var controller = new UserController(useCaseMock.Object, default!, default!, default!, default!);
        var command = new CreateUserCommand("John", "John123", new EmailAddress("john@doe.com"));
        
        // Act
        var response = await controller.CreateUserAsync(command, CancellationToken.None);
        
        // Assert
        var createdResult = Assert.IsType<CreatedAtRouteResult>(response);
        
        var value = Assert.IsType<UserDto>(createdResult.Value);
        Assert.Equal(expectedResult.Email, value.Email);
    }

    [Fact]
    public async Task Create_shouldReturnBadRequest_WhenEmailIsAlreadyInUse()
    {
        // Arrange
        var useCaseMock = new Mock<CreateUserUseCase>(MockBehavior.Default, default!, default!, default!);
        var expectedError = "Email already in use.";
        
        useCaseMock
            .Setup(useCase => useCase.ExecuteAsync(It.IsAny<CreateUserCommand>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException(expectedError));

        var controller = new UserController(useCaseMock.Object, default!, default!, default!, default!);
        var command = new CreateUserCommand("John", "John123", new EmailAddress("john@doe.com"));
        
        // Act
        var response = await controller.CreateUserAsync(command, CancellationToken.None);
        
        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(response);
        Assert.Equal(expectedError, badRequestResult.Value);
    }
    
    [Fact]
    public async Task Create_shouldReturnBadRequest_WhenUsernameIsAlreadyInUse()
    {
        // Arrange
        var useCaseMock = new Mock<CreateUserUseCase>(MockBehavior.Default, default!, default!, default!);
        var expectedError = "Username already in use.";
        
        useCaseMock
            .Setup(useCase => useCase.ExecuteAsync(It.IsAny<CreateUserCommand>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException(expectedError));

        var controller = new UserController(useCaseMock.Object, default!, default!, default!, default!);
        var command = new CreateUserCommand("John", "John123", new EmailAddress("john@doe.com"));
        
        // Act
        var response = await controller.CreateUserAsync(command, CancellationToken.None);
        
        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(response);
        Assert.Equal(expectedError, badRequestResult.Value);
    }

    [Fact]
    public async Task Read_shouldReturnOk_WhenUseCaseSucceeds()
    {
        // Arrange
        var useCaseMock = new Mock<ReadUserUseCase>(MockBehavior.Default, default!);
        var expectedResult = new ReadUserResult(Guid.NewGuid(), "John", new EmailAddress("john@doe.com"));
        
        useCaseMock
            .Setup(useCase => useCase.ExecuteAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        var controller = new UserController(default!, useCaseMock.Object, default!, default!, default!);
        
        // Act
        var response = await controller.ReadUserAsync(expectedResult.Id, CancellationToken.None);
        
        // Assert
        var readResult = Assert.IsType<OkObjectResult>(response);
        
        var value = Assert.IsType<UserDto>(readResult.Value);
        Assert.Equal(expectedResult.Email, value.Email);
    }

    [Fact]
    public async Task Read_shouldReturnNotFound_WhenUserDoesNotExist()
    {
        // Arrange
        var useCaseMock = new Mock<ReadUserUseCase>(MockBehavior.Default, default!);
        var expectedError = "User not found.";
        
        useCaseMock
            .Setup(useCase => useCase.ExecuteAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new KeyNotFoundException(expectedError));
        
        var controller = new UserController(default!, useCaseMock.Object, default!, default!, default!);
        
        // Act
        var response = await controller.ReadUserAsync(Guid.NewGuid(), CancellationToken.None);
        
        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(response);
        Assert.Equal(expectedError, notFoundResult.Value);
    }
}