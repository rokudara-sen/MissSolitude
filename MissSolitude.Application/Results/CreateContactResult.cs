using MissSolitude.Domain.ValueObjects;

namespace MissSolitude.Application.Results;

public sealed record CreateContactResult(Guid Id, string FirstName, string LastName, EmailAddress? Email = default!, string? Phone = default!, string? Notes = default!);