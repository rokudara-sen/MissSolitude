using MissSolitude.Domain.Entities;

namespace MissSolitude.Application.Interfaces.Repositories;

public interface IContactRepository
{
    Task<bool> FirstAndLastNameExistAsync(string firstName, string lastName, CancellationToken cancellationToken);

    Task<Contact?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<Contact?> GetByFirstAndLastNameAsync(string firstName, string lastName, CancellationToken cancellationToken);

    Task AddAsync(Contact contact, CancellationToken cancellationToken);
    void Remove(Contact contact);
}