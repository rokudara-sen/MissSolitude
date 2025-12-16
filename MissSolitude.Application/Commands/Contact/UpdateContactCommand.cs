using MissSolitude.Domain.ValueObjects;

namespace MissSolitude.Application.Commands.Contact;

public sealed record UpdateContactCommand(Guid Id, string FirstName, string LastName, EmailAddress? Email = default!, string? Phone = default!, string? Notes = default!);