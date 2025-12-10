using MissSolitude.Domain.ValueObjects;

namespace MissSolitude.Application.Commands;

public sealed record TokenUser(Guid Id, string Username, EmailAddress Email);