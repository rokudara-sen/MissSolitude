using MissSolitude.Domain.ValueObjects;

namespace MissSolitude.Application.Results;

public record ReadUserResult(Guid Id, string Username, EmailAddress Email);