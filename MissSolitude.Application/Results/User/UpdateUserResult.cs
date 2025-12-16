using MissSolitude.Domain.ValueObjects;

namespace MissSolitude.Application.Results.User;

public sealed record UpdateUserResult(Guid Id, string Username, EmailAddress Email);