using MissSolitude.Domain.ValueObjects;

namespace MissSolitude.Contracts.Contact;

public record CreateContactRequest(string FirstName, string LastName, EmailAddress? Email = default!, string? Phone = default!, string? Notes = default!);