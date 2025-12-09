using MissSolitude.Domain.ValueObjects;

namespace MissSolitude.Application.Results;

public sealed record UpdateUserResult(Guid Id, string Username, EmailAddress Email);