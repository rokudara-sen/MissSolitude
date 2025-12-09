using Microsoft.EntityFrameworkCore;
using MissSolitude.Application.Commands;
using MissSolitude.Application.Results;
using MissSolitude.Application.Services.Interfaces;
using MissSolitude.Domain.Entities;

namespace MissSolitude.Infrastructure.Services;

public class UserService : IUserService
{
    private readonly DatabaseContext _databaseContext;
    private readonly IPasswordHasher _passwordHasher;

    public UserService(DatabaseContext databaseContext, IPasswordHasher passwordHasher)
    {
        _databaseContext = databaseContext;
        _passwordHasher = passwordHasher;
    }

    public async Task<CreateUserResult> CreateAsync(CreateUserCommand request, CancellationToken cancellationToken)
    {
        var exists = await _databaseContext.Users
            .AnyAsync(user => user.Email == request.Email, cancellationToken);

        if (exists)
            throw new InvalidOperationException("User already exists.");

        var hash = _passwordHasher.Hash(request.Password);

        var user = new User(
            id: Guid.NewGuid(),
            username: request.Username.Trim(),
            passwordHash: hash,
            email: request.Email
        );

        _databaseContext.Users.Add(user);
        await _databaseContext.SaveChangesAsync(cancellationToken);

        return new CreateUserResult(user.Id, user.Username, user.Email);
    }
    
    public async Task<DeleteUserResult> DeleteAsync(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        var existingUser = await _databaseContext.Users.FindAsync([request.Id], cancellationToken);

        if (existingUser is null)
            throw new KeyNotFoundException($"User with id '{request.Id}' does not exist.");

        _databaseContext.Users.Remove(existingUser);
        await _databaseContext.SaveChangesAsync(cancellationToken);

        return new DeleteUserResult(existingUser.Id);
    }
}
