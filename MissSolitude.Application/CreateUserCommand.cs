using MissSolitude.Domain;

namespace MissSolitude.Application;

public sealed record CreateUserCommand(string Username, string Password, EmailAddress Email);