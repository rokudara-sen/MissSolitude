using MissSolitude.Domain;

namespace MissSolitude.Contracts;

public sealed record UserDto(
    Guid Id,
    string Username,
    EmailAddress Email);

public sealed record CreateUserRequest(string Username,
    string Password,
    EmailAddress Email);