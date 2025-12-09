using Microsoft.EntityFrameworkCore;
using MissSolitude.Application;
using MissSolitude.Application.Services;
using MissSolitude.Domain;
using MissSolitude.Domain.Entities;

namespace MissSolitude.Infrastructure;

public class UserService : IUserService
{
    private readonly DatabaseContext _db;
    private readonly IPasswordHasher _passwordHasher;

    public UserService(DatabaseContext db, IPasswordHasher passwordHasher)
    {
        _db = db;
        _passwordHasher = passwordHasher;
    }

    public async Task<CreateUserResult> CreateAsync(CreateUserCommand request, CancellationToken cancellationToken)
    {
        var exists = await _db.Users
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

        _db.Users.Add(user);
        await _db.SaveChangesAsync(cancellationToken);

        return new CreateUserResult(user.Id, user.Username, user.Email);
    }
    
    public async Task<RemoveUserResult> RemoveAsync(RemoveUserCommand request, CancellationToken cancellationToken)
    {
        var existingUser = await _db.Users.FindAsync([request.Id], cancellationToken);

        if (existingUser is null)
            throw new KeyNotFoundException($"User with id '{request.Id}' does not exist.");

        _db.Users.Remove(existingUser);
        await _db.SaveChangesAsync(cancellationToken);

        return new RemoveUserResult(existingUser.Id);
    }
}
