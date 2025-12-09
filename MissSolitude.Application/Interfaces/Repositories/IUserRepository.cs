using MissSolitude.Domain.Entities;
using MissSolitude.Domain.ValueObjects;

namespace MissSolitude.Application.Interfaces.Repositories;

public interface IUserRepository
{
    Task<bool> EmailExistsAsync(EmailAddress email, CancellationToken cancellationToken);
    Task<bool> UsernameExistsAsync(string username, CancellationToken cancellationToken);
    
    Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<User?> GetByEmailOrUsernameAsync(string identifier, CancellationToken cancellationToken);
    
    Task AddAsync(User user, CancellationToken cancellationToken);
    void Remove(User user);
}