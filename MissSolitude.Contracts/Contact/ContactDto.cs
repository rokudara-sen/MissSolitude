using MissSolitude.Domain.ValueObjects;

namespace MissSolitude.Contracts.Contact;

public record ContactDto(Guid Id, string FirstName, string LastName, EmailAddress? Email, string? Phone, string? Notes);