using MissSolitude.Domain.ValueObjects;

namespace MissSolitude.Contracts.Contact;

public record ContactDto(Guid Id, string FirstName, string LastName, string Phone, EmailAddress Email, string Notes);