using MissSolitude.Domain;

namespace MissSolitude.Application;

public sealed record CreateUserResult(Guid Id, string Username, EmailAddress Email);