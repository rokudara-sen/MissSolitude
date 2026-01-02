using IntegrationTests.Fixtures;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using MissSolitude.Domain.Entities;
using MissSolitude.Domain.ValueObjects;
using MissSolitude.Infrastructure.Repositories;

namespace IntegrationTests.Repositories;

[TestSubject(typeof(ContactRepository))]
public class ContactRepositoryTest : IClassFixture<PostgresFixture>, IAsyncLifetime
{
    private readonly PostgresFixture _fixture;
    private readonly ContactRepository _contactRepository;

    public ContactRepositoryTest(PostgresFixture fixture)
    {
        _fixture = fixture;
        _contactRepository = new ContactRepository(_fixture.Context);
    }

    public async Task InitializeAsync()
    {
        await _fixture.ResetDatabaseAsync();
    }

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task FirstAndLastNameExistAsync_ShouldReturnTrue_WhenContactExists()
    {
        // Arrange
        var contact = new Contact(
            Guid.NewGuid(),
            "John",
            "Doe",
            new EmailAddress("john@doe.com"),
            "123-456",
            "Test Note"
        );

        await _fixture.Context.Contacts.AddAsync(contact);
        await _fixture.Context.SaveChangesAsync();

        _fixture.Context.ChangeTracker.Clear();

        // Act
        var exists = await _contactRepository.FirstAndLastNameExistAsync("John", "Doe", CancellationToken.None);

        // Assert
        Assert.True(exists);
    }

    [Fact]
    public async Task FirstAndLastNameExistAsync_ShouldReturnFalse_WhenContactDoesNotExist()
    {
        // Act
        var exists = await _contactRepository.FirstAndLastNameExistAsync("Ghost", "User", CancellationToken.None);

        // Assert
        Assert.False(exists);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnContact_WhenContactExists()
    {
        // Arrange
        var id = Guid.NewGuid();
        var email = new EmailAddress("jane@doe.com");
        var contact = new Contact(
            id,
            "Jane",
            "Doe",
            email,
            "987-654",
            "Another Note"
        );

        await _fixture.Context.Contacts.AddAsync(contact);
        await _fixture.Context.SaveChangesAsync();

        _fixture.Context.ChangeTracker.Clear();

        // Act
        var result = await _contactRepository.GetByIdAsync(id, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(id, result.Id);
        Assert.Equal("Jane", result.FirstName);
        Assert.Equal("Doe", result.LastName);
        Assert.Equal(email, result.Email);
        Assert.Equal("987-654", result.Phone);
        Assert.Equal("Another Note", result.Notes);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNull_WhenContactDoesNotExist()
    {
        // Act
        var result = await _contactRepository.GetByIdAsync(Guid.NewGuid(), CancellationToken.None);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetByFirstAndLastNameAsync_ShouldReturnContact_WhenContactExists()
    {
        // Arrange
        var contact = new Contact(
            Guid.NewGuid(),
            "Alice",
            "Wonderland",
            new EmailAddress("alice@test.com"),
            "555-0100",
            "Down the rabbit hole"
        );

        await _fixture.Context.Contacts.AddAsync(contact);
        await _fixture.Context.SaveChangesAsync();

        _fixture.Context.ChangeTracker.Clear();

        // Act
        var result = await _contactRepository.GetByFirstAndLastNameAsync("Alice", "Wonderland", CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(contact.Id, result.Id);
    }

    [Fact]
    public async Task GetByFirstAndLastNameAsync_ShouldReturnNull_WhenContactDoesNotExist()
    {
        // Act
        var result = await _contactRepository.GetByFirstAndLastNameAsync("Nobody", "Here", CancellationToken.None);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task AddAsync_ShouldPersistContactToDatabase()
    {
        // Arrange
        var newContact = new Contact(
            Guid.NewGuid(),
            "Bob",
            "Builder",
            new EmailAddress("bob@construction.com"),
            "555-FIXIT",
            "Can we fix it?"
        );

        // Act
        await _contactRepository.AddAsync(newContact, CancellationToken.None);
        await _fixture.Context.SaveChangesAsync();

        _fixture.Context.ChangeTracker.Clear();

        // Assert
        var dbContact = await _fixture.Context.Contacts.FirstOrDefaultAsync(contactContext => contactContext.Id == newContact.Id);
        Assert.NotNull(dbContact);
        Assert.Equal("Bob", dbContact.FirstName);
    }

    [Fact]
    public async Task Remove_ShouldDeleteContactFromDatabase()
    {
        // Arrange
        var contact = new Contact(
            Guid.NewGuid(),
            "Delete",
            "Me",
            new EmailAddress("delete@test.com"),
            "000-000",
            "Goodbye"
        );

        await _fixture.Context.Contacts.AddAsync(contact);
        await _fixture.Context.SaveChangesAsync();

        _fixture.Context.ChangeTracker.Clear();

        // Act
        var contactToDelete = await _fixture.Context.Contacts.FirstAsync(contactContext => contactContext.Id == contact.Id);

        _contactRepository.Remove(contactToDelete);
        await _fixture.Context.SaveChangesAsync();

        _fixture.Context.ChangeTracker.Clear();

        // Assert
        var dbContact = await _fixture.Context.Contacts.AsNoTracking().FirstOrDefaultAsync(contactContext => contactContext.Id == contact.Id);
        Assert.Null(dbContact);
    }

    // TODO: Add tests for listing all contacts
}