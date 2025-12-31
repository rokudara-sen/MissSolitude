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

        var controller = CreateController(create: useCaseMock.Object);
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

        var controller = CreateController(create: useCaseMock.Object);
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

        var controller = CreateController(create: useCaseMock.Object);
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

        var controller = CreateController(read: useCaseMock.Object);

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

        var controller = CreateController(read: useCaseMock.Object);

        // Act
        var response = await controller.ReadUserAsync(Guid.NewGuid(), CancellationToken.None);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(response);
        Assert.Equal(expectedError, notFoundResult.Value);
    }

    [Fact]
    public async Task Update_shouldReturnOk_WhenUseCaseSucceeds()
    {
        // Arrange
        var useCaseMock = new Mock<UpdateUserUseCase>(MockBehavior.Default, default!, default!, default!);
        var expectedResult = new ReadUserResult(Guid.NewGuid(), "John", new EmailAddress("john@doe.com"));

        useCaseMock
            .Setup(useCase => useCase.ExecuteAsync(It.IsAny<UpdateUserCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        var controller = CreateController(update: useCaseMock.Object);
        var command = new UpdateUserCommand(expectedResult.Id, "John", "John123", new EmailAddress("john@doe.com"));

        // Act
        var response = await controller.UpdateUserAsync(expectedResult.Id, command, CancellationToken.None);

        // Assert
        var updateResult = Assert.IsType<OkObjectResult>(response);
        var value = Assert.IsType<UserDto>(updateResult.Value);

        Assert.Equal(expectedResult.Email, value.Email);

        useCaseMock.Verify(useCase => useCase.ExecuteAsync(command, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Update_shouldReturnBadRequest_WhenRouteIdDoesNotMatchBodyId()
    {
        // Arrange
        var useCaseMock = new Mock<UpdateUserUseCase>(MockBehavior.Default, default!, default!, default!);
        var controller = CreateController(update: useCaseMock.Object);

        var routeId = Guid.NewGuid();
        var differentBodyId = Guid.NewGuid();

        var command = new UpdateUserCommand(differentBodyId, "John", "John123", new EmailAddress("john@doe.com"));

        // Act
        var response = await controller.UpdateUserAsync(routeId, command, CancellationToken.None);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(response);
        Assert.Equal("Body id and route id do not match.", badRequestResult.Value);

        useCaseMock.Verify(x => x.ExecuteAsync(It.IsAny<UpdateUserCommand>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Update_shouldReturnNotFound_WhenUserDoesNotExist()
    {
        // Arrange
        var useCaseMock = new Mock<UpdateUserUseCase>(MockBehavior.Default, default!, default!, default!);
        var expectedError = "User not found.";

        var userId = Guid.NewGuid();

        useCaseMock
            .Setup(useCase => useCase.ExecuteAsync(It.IsAny<UpdateUserCommand>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new KeyNotFoundException(expectedError));

        var controller = CreateController(update: useCaseMock.Object);
        var command = new UpdateUserCommand(userId, "John", "John123", new EmailAddress("john@doe.com"));

        // Act
        var response = await controller.UpdateUserAsync(userId, command, CancellationToken.None);

        // Arrange
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(response);
        Assert.Equal(expectedError, notFoundResult.Value);
    }

    [Fact]
    public async Task Delete_shouldReturnNoContent_WhenUseCaseSucceeds()
    {
        // Arrange
        var useCaseMock = new Mock<DeleteUserUseCase>(MockBehavior.Default, default!, default!);
        var userId = Guid.NewGuid();

        useCaseMock
            .Setup(useCase => useCase.ExecuteAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var controller = CreateController(delete: useCaseMock.Object);

        // Act
        var response = await controller.DeleteUserAsync(userId, CancellationToken.None);

        // Assert
        Assert.IsType<NoContentResult>(response);
        useCaseMock.Verify(useCase => useCase.ExecuteAsync(userId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Delete_shouldReturnNotFound_WhenUserDoesNotExist()
    {
        // Arrange
        var useCaseMock = new Mock<DeleteUserUseCase>(MockBehavior.Default, default!, default!);
        var expectedError = "User not found.";
        var userId = Guid.NewGuid();

        useCaseMock
            .Setup(useCase => useCase.ExecuteAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new KeyNotFoundException(expectedError));

        var controller = CreateController(delete: useCaseMock.Object);

        // Act
        var response = await controller.DeleteUserAsync(userId, CancellationToken.None);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(response);
        Assert.Equal(expectedError, notFoundResult.Value);

        useCaseMock.Verify(useCase => useCase.ExecuteAsync(userId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Login_shouldReturnOk_WhenUseCaseSucceeds()
    {
        // Arrange
        var useCaseMock = new Mock<LogInUserUseCase>(MockBehavior.Default, default!, default!, default!);

        var userId = Guid.NewGuid();
        var email = new EmailAddress("john@doe.com");
        var userResult = new ReadUserResult(userId, "JohnDoe", email);

        var expectedResult = new LogInUserResult("fake_access_token_123", "fake_refresh_token_456", userResult);

        useCaseMock
            .Setup(x => x.LogInAsync(It.IsAny<LogInUserCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        var controller = CreateController(login: useCaseMock.Object);
        var command = new LogInUserCommand("john@doe.com", "Password123!");

        // Act
        var response = await controller.LoginUserAsync(command, CancellationToken.None);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(response);
        var value = Assert.IsType<LogInUserResult>(okResult.Value);

        Assert.Equal(expectedResult.AccessToken, value.AccessToken);
        Assert.Equal(expectedResult.RefreshToken, value.RefreshToken);
        Assert.Equal(expectedResult.User.Email, value.User.Email);

        useCaseMock.Verify(x => x.LogInAsync(command, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Login_shouldReturnUnauthorized_WhenCredentialsAreInvalid()
    {
        // Arrange
        var useCaseMock = new Mock<LogInUserUseCase>(MockBehavior.Default, default!, default!, default!);

        useCaseMock
            .Setup(x => x.LogInAsync(It.IsAny<LogInUserCommand>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new UnauthorizedAccessException("Invalid credentials."));

        var controller = CreateController(login: useCaseMock.Object);
        var command = new LogInUserCommand("wrong@email.com", "WrongPass");

        // Act
        var response = await controller.LoginUserAsync(command, CancellationToken.None);

        // Assert
        var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(response);
        Assert.Equal("Invalid credentials.", unauthorizedResult.Value);
    }

    private UserController CreateController(
        CreateUserUseCase? create = null,
        ReadUserUseCase? read = null,
        UpdateUserUseCase? update = null,
        DeleteUserUseCase? delete = null,
        LogInUserUseCase? login = null,
        RegisterUserUseCase? register = null)
    {
        return new UserController(
            create ?? new Mock<CreateUserUseCase>(MockBehavior.Default, default!, default!, default!).Object,
            read ?? new Mock<ReadUserUseCase>(MockBehavior.Default, default!).Object,
            update ?? new Mock<UpdateUserUseCase>(MockBehavior.Default, default!, default!, default!).Object,
            delete ?? new Mock<DeleteUserUseCase>(MockBehavior.Default, default!, default!).Object,
            login ?? new Mock<LogInUserUseCase>(MockBehavior.Default, default!, default!, default!).Object,
            register ?? new Mock<RegisterUserUseCase>(MockBehavior.Default, default!, default!, default!).Object
        );
    }
}