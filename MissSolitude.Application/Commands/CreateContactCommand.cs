using MissSolitude.Domain.ValueObjects;

namespace MissSolitude.Application.Commands;

public sealed record CreateContactCommand(string FirstName, string LastName, EmailAddress? Email = default!, string? Phone = default!, string? Notes = default!);