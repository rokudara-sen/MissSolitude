using MissSolitude.Application.Commands;
using MissSolitude.Application.Commands.User;
using MissSolitude.Application.Interfaces.Functions;
using MissSolitude.Application.Interfaces.Repositories;
using MissSolitude.Application.Results;
using MissSolitude.Application.Results.User;

namespace MissSolitude.Application.UseCases.User;

public class UpdateUserUseCase
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher _passwordHasher;
    
    public UpdateUserUseCase (IUserRepository userRepository, IUnitOfWork unitOfWork, IPasswordHasher passwordHasher)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _passwordHasher = passwordHasher;
    }

    public virtual async Task<ReadUserResult> ExecuteAsync(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        var existingUser = await _userRepository.GetByIdAsync(request.Id, cancellationToken);
        
        if(existingUser is null)
            throw new KeyNotFoundException("User not found.");
        
        existingUser.Username = request.Username.Trim();
        if(!string.IsNullOrWhiteSpace(request.Password)) existingUser.PasswordHash = _passwordHasher.Hash(request.Password);
        existingUser.Email = request.Email;
        
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return new ReadUserResult(request.Id, existingUser.Username, existingUser.Email);
    }
}