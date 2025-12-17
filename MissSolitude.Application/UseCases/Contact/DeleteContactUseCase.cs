using MissSolitude.Application.Interfaces.Functions;
using MissSolitude.Application.Interfaces.Repositories;

namespace MissSolitude.Application.UseCases.Contact;

public class DeleteContactUseCase
{
    private readonly IContactRepository _contactRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteContactUseCase(IContactRepository contactRepository, IUnitOfWork unitOfWork)
    {
        _contactRepository = contactRepository;
        _unitOfWork = unitOfWork;
    }

    public virtual async Task ExecuteAsync(Guid id, CancellationToken cancellationToken)
    {
        var existingContact = await _contactRepository.GetByIdAsync(id, cancellationToken);

        if (existingContact is null)
            throw new KeyNotFoundException("Contact not found.");

        _contactRepository.Remove(existingContact);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}