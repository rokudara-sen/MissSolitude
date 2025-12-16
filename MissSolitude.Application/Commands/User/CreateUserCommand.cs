using MissSolitude.Domain.ValueObjects;

namespace MissSolitude.Application.Commands.User;

public sealed record CreateUserCommand(string Username, string Password, EmailAddress Email);