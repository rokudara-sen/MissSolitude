using MissSolitude.Domain;
using MissSolitude.Domain.ValueObjects;

namespace MissSolitude.Application;

public sealed record CreateUserResult(Guid Id, string Username, EmailAddress Email);