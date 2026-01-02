using MissSolitude.Application.Interfaces.Repositories;
using MissSolitude.Application.Results.Contact;

namespace MissSolitude.Application.UseCases.Contact;

public class ListContactsUseCase
{
    private readonly IContactRepository _contactRepository;

    public ListContactsUseCase(IContactRepository contactRepository)
    {
        _contactRepository = contactRepository;
    }

    public virtual async Task<IReadOnlyList<ReadContactResult>> ExecuteAsync(CancellationToken cancellationToken)
    {
        var contacts = await _contactRepository.GetAllAsync(cancellationToken);

        if (contacts is null)
            throw new KeyNotFoundException("No Contacts.");

        return contacts
            .Select(contact => new ReadContactResult(contact.Id, contact.FirstName, contact.LastName, contact.Email, contact.Phone, contact.Notes))
            .ToList();
    }
}