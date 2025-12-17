using IntegrationTests.Fixtures;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using MissSolitude.Domain.Entities;
using MissSolitude.Domain.ValueObjects;
using MissSolitude.Infrastructure.Repositories;

namespace IntegrationTests.Repositories;

[TestSubject(typeof(UserRepository))]
public class UserRepositoryTest : IClassFixture<PostgresFixture>, IAsyncLifetime
{
    private readonly PostgresFixture _fixture;
    private readonly UserRepository _userRepository;

    public UserRepositoryTest(PostgresFixture fixture)
    {
        _fixture = fixture;
        _userRepository = new UserRepository(_fixture.Context);
    }

    public async Task InitializeAsync()
    {
        await _fixture.ResetDatabaseAsync();
    }

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task GetByEmailOrUsernameAsync_ShouldReturnUser_WhenInputIsEmail()
    {
        // Arrange
        var expectedUser = new User(Guid.NewGuid(), "John", "john_password_hash", new EmailAddress("john@example.com"));

        await _fixture.Context.Users.AddAsync(expectedUser);
        await _fixture.Context.SaveChangesAsync();

        _fixture.Context.ChangeTracker.Clear();

        // Act
        var result = await _userRepository.GetByEmailOrUsernameAsync("john@example.com", CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedUser.Id, result.Id);
    }

    [Fact]
    public async Task GetByEmailOrUsernameAsync_ShouldReturnUser_WhenInputIsUsername()
    {
        // Arrange
        var expectedUser = new User(Guid.NewGuid(), "random", "jane_doe_password_hash", new EmailAddress("jane@example.com"));

        await _fixture.Context.Users.AddAsync(expectedUser);
        await _fixture.Context.SaveChangesAsync();

        _fixture.Context.ChangeTracker.Clear();

        // Act
        var result = await _userRepository.GetByEmailOrUsernameAsync("random", CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedUser.Id, result.Id);
    }

    [Fact]
    public async Task GetByEmailOrUsernameAsync_ShouldReturnNull_WhenNoMatch()
    {
        var result = await _userRepository.GetByEmailOrUsernameAsync("missing@example.com", CancellationToken.None);
        Assert.Null(result);
    }

    [Fact]
    public async Task AddAsync_ShouldPersistUserToDatabase()
    {
        // Arrange
        var newUser = new User(Guid.NewGuid(), "new_user", "new_password_hash", new EmailAddress("new@test.com"));

        // Act
        await _userRepository.AddAsync(newUser, CancellationToken.None);
        await _fixture.Context.SaveChangesAsync();

        _fixture.Context.ChangeTracker.Clear();

        // Assert
        var dbUser = await _fixture.Context.Users.FirstOrDefaultAsync(u => u.Id == newUser.Id);
        Assert.NotNull(dbUser);
    }

    [Fact]
    public async Task EmailExistsAsync_ShouldReturnTrue_WhenEmailExists()
    {
        // Arrange
        var email = new EmailAddress("exists@test.com");
        var user = new User(Guid.NewGuid(), "exists_user", "hash", email);

        await _fixture.Context.Users.AddAsync(user);
        await _fixture.Context.SaveChangesAsync();

        _fixture.Context.ChangeTracker.Clear();

        // Act
        var exists = await _userRepository.EmailExistsAsync(email, CancellationToken.None);

        // Assert
        Assert.True(exists);
    }

    [Fact]
    public async Task EmailExistsAsync_ShouldReturnFalse_WhenEmailDoesNotExist()
    {
        // Act
        var exists = await _userRepository.EmailExistsAsync(new EmailAddress("ghost@test.com"), CancellationToken.None);

        // Assert
        Assert.False(exists);
    }

    [Fact]
    public async Task UsernameExistsAsync_ShouldReturnTrue_WhenUsernameExists()
    {
        // Arrange
        var user = new User(Guid.NewGuid(), "unique_user", "hash", new EmailAddress("unique@test.com"));

        await _fixture.Context.Users.AddAsync(user);
        await _fixture.Context.SaveChangesAsync();

        _fixture.Context.ChangeTracker.Clear();

        // Act
        var exists = await _userRepository.UsernameExistsAsync("unique_user", CancellationToken.None);

        // Assert
        Assert.True(exists);
    }

    [Fact]
    public async Task UsernameExistsAsync_ShouldReturnFalse_WhenUsernameDoesNotExist()
    {
        // Act
        var exists = await _userRepository.UsernameExistsAsync("missing_user", CancellationToken.None);

        // Assert
        Assert.False(exists);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnUser_WhenUserExists()
    {
        // Arrange
        var id = Guid.NewGuid();
        var email = new EmailAddress("find@test.com");
        var username = "find_me";
        var user = new User(id, username, "hash", email);

        await _fixture.Context.Users.AddAsync(user);
        await _fixture.Context.SaveChangesAsync();

        _fixture.Context.ChangeTracker.Clear();

        // Act
        var result = await _userRepository.GetByIdAsync(id, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(id, result.Id);
        Assert.Equal(username, result.Username);
        Assert.Equal(email, result.Email);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNull_WhenUserDoesNotExist()
    {
        // Act
        var result = await _userRepository.GetByIdAsync(Guid.NewGuid(), CancellationToken.None);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task Remove_ShouldDeleteUserFromDatabase()
    {
        // Arrange
        var user = new User(Guid.NewGuid(), "delete_me", "hash", new EmailAddress("delete@test.com"));

        await _fixture.Context.Users.AddAsync(user);
        await _fixture.Context.SaveChangesAsync();
        _fixture.Context.ChangeTracker.Clear();

        // Act
        var userToDelete = await _fixture.Context.Users.FirstAsync(u => u.Id == user.Id);

        _userRepository.Remove(userToDelete);
        await _fixture.Context.SaveChangesAsync();

        _fixture.Context.ChangeTracker.Clear();

        // Assert
        var dbUser = await _fixture.Context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == user.Id);
        Assert.Null(dbUser);
    }
}