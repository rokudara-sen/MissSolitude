using MissSolitude.Application.Commands.Contact;
using MissSolitude.Application.Interfaces.Functions;
using MissSolitude.Application.Interfaces.Repositories;
using MissSolitude.Application.Results.Contact;

namespace MissSolitude.Application.UseCases.Contact;

public class UpdateContactUseCase
{
    private readonly IContactRepository _contactRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateContactUseCase(IContactRepository contactRepository, IUnitOfWork unitOfWork)
    {
        _contactRepository = contactRepository;
        _unitOfWork = unitOfWork;
    }

    public virtual async Task<ReadContactResult> ExecuteAsync(UpdateContactCommand request, CancellationToken cancellationToken)
    {
        var existingContact = await _contactRepository.GetByIdAsync(request.Id, cancellationToken);

        if (existingContact is null)
            throw new KeyNotFoundException("Contact not found.");

        existingContact.FirstName = request.FirstName;
        existingContact.LastName = request.LastName;
        existingContact.Email = request.Email;
        existingContact.Phone = request.Phone;
        existingContact.Notes = request.Notes;

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return new ReadContactResult(request.Id, existingContact.FirstName, existingContact.LastName, existingContact.Email, existingContact.Phone, existingContact.Notes);
    }
}