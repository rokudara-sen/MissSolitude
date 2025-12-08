using System.Data;
using Microsoft.EntityFrameworkCore;
using MissSolitude.Application;
using MissSolitude.Domain;

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

    public async Task<CreateUserResult> CreateAsync(CreateUserCommand request)
    {
        var exists = await _db.Users.AnyAsync(user => user.Email == request.Email);
        if (exists) 
            throw new DuplicateNameException("User already exists.");

        var hash = _passwordHasher.Hash(request.Password);

        var user = new User(Guid.NewGuid(), request.Username, hash, request.Email);
        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        return new CreateUserResult(user.Id, user.Username, user.Password, user.Email);
    }
}
