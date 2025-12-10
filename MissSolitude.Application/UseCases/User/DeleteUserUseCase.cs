using MissSolitude.Application.Interfaces.Functions;
using MissSolitude.Application.Interfaces.Repositories;

namespace MissSolitude.Application.UseCases.User;

public class DeleteUserUseCase
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    
    public DeleteUserUseCase(IUserRepository userRepository, IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task ExecuteAsync(Guid id, CancellationToken cancellationToken)
    {
        var existingUser = await _userRepository.GetByIdAsync(id, cancellationToken);
        
        if(existingUser is null)
            throw new KeyNotFoundException("User not found.");
        
        _userRepository.Remove(existingUser);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}