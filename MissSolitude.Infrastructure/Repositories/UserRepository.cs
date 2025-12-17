using Microsoft.EntityFrameworkCore;
using MissSolitude.Application.Interfaces.Repositories;
using MissSolitude.Domain.Entities;
using MissSolitude.Domain.ValueObjects;

namespace MissSolitude.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly DatabaseContext _databaseContext;

    public UserRepository(DatabaseContext databaseContext)
    {
        _databaseContext = databaseContext;
    }

    public Task<bool> EmailExistsAsync(EmailAddress email, CancellationToken cancellationToken)
    {
        return _databaseContext.Users.AnyAsync(user => user.Email == email, cancellationToken);
    }

    public Task<bool> UsernameExistsAsync(string username, CancellationToken cancellationToken)
    {
        return _databaseContext.Users.AnyAsync(user => user.Username == username, cancellationToken);
    }

    public Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return _databaseContext.Users.SingleOrDefaultAsync(user => user.Id == id, cancellationToken);
    }

    public Task<User?> GetByEmailOrUsernameAsync(string identifier, CancellationToken cancellationToken)
    {
        if (identifier.Contains('@'))
        {
            var email = new EmailAddress(identifier);
            return _databaseContext.Users.SingleOrDefaultAsync(
                user => user.Username == identifier || user.Email == email,
                cancellationToken);
        }

        return _databaseContext.Users.SingleOrDefaultAsync(
            user => user.Username == identifier,
            cancellationToken);
    }

    public Task AddAsync(User user, CancellationToken cancellationToken)
    {
        return _databaseContext.Users.AddAsync(user, cancellationToken).AsTask();
    }

    public void Remove(User user)
    {
        _databaseContext.Users.Remove(user);
    }
}