using MissSolitude.Domain.ValueObjects;

namespace MissSolitude.Application.Commands;

public sealed record UpdateUserCommand(Guid Id, string Username, string Password, EmailAddress Email);