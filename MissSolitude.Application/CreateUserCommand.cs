using MissSolitude.Domain;
using MissSolitude.Domain.ValueObjects;

namespace MissSolitude.Application;

public sealed record CreateUserCommand(string Username, string Password, EmailAddress Email);