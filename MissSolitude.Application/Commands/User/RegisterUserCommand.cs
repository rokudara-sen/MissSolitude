using MissSolitude.Domain.ValueObjects;

namespace MissSolitude.Application.Commands.User;

public sealed record RegisterUserCommand(string Username, string Password, EmailAddress Email);