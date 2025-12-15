using MissSolitude.Application.Commands;
using MissSolitude.Application.Interfaces.Functions;
using MissSolitude.Application.Interfaces.Repositories;
using MissSolitude.Application.Results;

namespace MissSolitude.Application.UseCases.Contact;

public class CreateContactUseCase
{
    private readonly IContactRepository _contactRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateContactUseCase(IContactRepository contactRepository, IUnitOfWork unitOfWork)
    {
        _contactRepository = contactRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<CreateContactResult> ExecuteAsync(CreateContactCommand request, CancellationToken cancellationToken)
    {
        if(await _contactRepository.FirstAndLastNameExistAsync(request.FirstName, request.LastName, cancellationToken))
            throw new InvalidOperationException("Contact already exists.");

        var newContact = new Domain.Entities.Contact(
            id: Guid.NewGuid(),
            firstName: request.FirstName, 
            lastName: request.LastName,
            email: request.Email,
            phone: request.Phone,
            notes: request.Notes
        );

        await _contactRepository.AddAsync(newContact, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        return new CreateContactResult(newContact.Id, newContact.FirstName, newContact.LastName, newContact.Email, newContact.Phone, newContact.Notes);
    }
}