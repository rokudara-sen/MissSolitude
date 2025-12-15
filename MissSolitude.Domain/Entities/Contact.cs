using MissSolitude.Domain.ValueObjects;

namespace MissSolitude.Domain.Entities;

public class Contact
{
    public Guid Id { get; set; }
    public string FirstName { get; set; } = default!;
    public string LastName { get; set; } = default!;
    public string? Phone { get; set; }
    public EmailAddress? Email { get; set; }
    public string? Notes { get; set; }

    public Contact()
    {
        
    }
    
    public Contact(Guid id, string firstName, string lastName, string? phone = default!, EmailAddress? email = default!, string? notes = default!)
    {
        Id = id;
        FirstName = firstName;
        LastName = lastName;
        Phone = phone;
        Email = email;
        Notes = notes;
    }
}