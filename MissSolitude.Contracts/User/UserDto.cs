using MissSolitude.Domain.ValueObjects;

namespace MissSolitude.Contracts.User;

public sealed record UserDto(
    Guid Id,
    string Username,
    EmailAddress Email);