using MissSolitude.Domain.ValueObjects;

namespace MissSolitude.Domain.Entities;

public class Contact
{
    public Guid Id { get; set; }
    public string FirstName { get; set; } = default!;
    public string LastName { get; set; } = default!;
    public EmailAddress? Email { get; set; }
    public string? Phone { get; set; }
    public string? Notes { get; set; }

    public Contact()
    {
        
    }
    
    public Contact(Guid id, string firstName, string lastName, EmailAddress? email = default!, string? phone = default!, string? notes = default!)
    {
        Id = id;
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        Phone = phone;
        Notes = notes;
    }
}