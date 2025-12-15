using Microsoft.EntityFrameworkCore;
using MissSolitude.Application.Interfaces.Repositories;
using MissSolitude.Domain.Entities;

namespace MissSolitude.Infrastructure.Repositories;

public class ContactRepository : IContactRepository
{
    private readonly DatabaseContext _databaseContext;

    public ContactRepository(DatabaseContext databaseContext)
    {
        _databaseContext = databaseContext;
    }
    
    public Task<bool> FirstAndLastNameExistAsync(string firstName, string lastName, CancellationToken cancellationToken)
    {
        return _databaseContext.Contacts.AnyAsync(existingContact => existingContact.FirstName == firstName && existingContact.LastName == lastName, cancellationToken);
    }

    public Task<Contact?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return _databaseContext.Contacts.SingleOrDefaultAsync(existingContact => existingContact.Id == id, cancellationToken);
    }

    public Task<Contact?> GetByFirstAndLastNameAsync(string firstName, string lastName, CancellationToken cancellationToken)
    {
        return _databaseContext.Contacts.SingleOrDefaultAsync(existingContact => existingContact.FirstName == firstName && existingContact.LastName == lastName, cancellationToken);
    }

    public Task AddAsync(Contact contact, CancellationToken cancellationToken)
    {
        return _databaseContext.Contacts.AddAsync(contact, cancellationToken).AsTask();
    }

    public void Remove(Contact contact)
    {
        _databaseContext.Contacts.Remove(contact);
    }
}