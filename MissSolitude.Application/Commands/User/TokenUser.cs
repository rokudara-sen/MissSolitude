using MissSolitude.Domain.ValueObjects;

namespace MissSolitude.Application.Commands.User;

public sealed record TokenUser(Guid Id, string Username, EmailAddress Email);