using MissSolitude.Application.Interfaces.Repositories;
using MissSolitude.Application.Results.Contact;

namespace MissSolitude.Application.UseCases.Contact;

public class ReadContactUseCase
{
    private readonly IContactRepository _contactRepository;

    public ReadContactUseCase(IContactRepository contactRepository)
    {
        _contactRepository = contactRepository;
    }

    public virtual async Task<ReadContactResult> ExecuteAsync(Guid id, CancellationToken cancellationToken)
    {
        var existingContact = await _contactRepository.GetByIdAsync(id, cancellationToken);

        if (existingContact is null)
            throw new KeyNotFoundException("Contact not found.");

        return new ReadContactResult(existingContact.Id, existingContact.FirstName, existingContact.LastName, existingContact.Email, existingContact.Phone, existingContact.Notes);
    }
}