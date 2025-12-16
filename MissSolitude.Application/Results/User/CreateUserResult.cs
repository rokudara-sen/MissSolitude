using MissSolitude.Domain.ValueObjects;

namespace MissSolitude.Application.Results.User;

public sealed record CreateUserResult(Guid Id, string Username, EmailAddress Email);