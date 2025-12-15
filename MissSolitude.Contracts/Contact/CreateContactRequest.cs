using MissSolitude.Domain.ValueObjects;

namespace MissSolitude.Contracts.Contact;

public record CreateContactRequest(string FirstName, string LastName, string? Phone = default!, EmailAddress? Email = default!, string? Notes = default!);