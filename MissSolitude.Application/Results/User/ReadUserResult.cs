using MissSolitude.Domain.ValueObjects;

namespace MissSolitude.Application.Results.User;

public record ReadUserResult(Guid Id, string Username, EmailAddress Email);