using Microsoft.AspNetCore.Identity;
using MissSolitude.Application.Interfaces.Functions;

namespace MissSolitude.Infrastructure.Services;

public class PasswordHasher : IPasswordHasher
{
    private static readonly object DummyUser = new();
    private readonly PasswordHasher<object> _hasher = new();

    public string Hash(string? password) =>
        _hasher.HashPassword(DummyUser, password);

    public bool Verify(string password, string passwordHash) =>
        _hasher.VerifyHashedPassword(DummyUser, passwordHash, password) != PasswordVerificationResult.Failed;
}