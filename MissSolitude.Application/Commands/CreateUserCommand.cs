using MissSolitude.Domain.ValueObjects;

namespace MissSolitude.Application.Commands;

public sealed record CreateUserCommand(string Username, string Password, EmailAddress Email);