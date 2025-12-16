using MissSolitude.Application.Commands;
using MissSolitude.Application.Commands.User;
using MissSolitude.Application.Interfaces.Functions;
using MissSolitude.Application.Interfaces.Repositories;
using MissSolitude.Application.Results;
using MissSolitude.Application.Results.User;

namespace MissSolitude.Application.UseCases.User;

public class CreateUserUseCase
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IUnitOfWork _unitOfWork;

    public CreateUserUseCase(IUserRepository userRepository, IPasswordHasher passwordHasher, IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _unitOfWork = unitOfWork;
    }

    public virtual async Task<CreateUserResult> ExecuteAsync(CreateUserCommand request, CancellationToken cancellationToken)
    {
        var username = request.Username.Trim();
        
        if(await _userRepository.EmailExistsAsync(request.Email, cancellationToken))
            throw new InvalidOperationException("User already exists.");
        
        if(await _userRepository.UsernameExistsAsync(username, cancellationToken))
            throw new InvalidOperationException("User already exists.");

        var newUser = new Domain.Entities.User(
            id: Guid.NewGuid(),
            username: username,
            passwordHash: _passwordHasher.Hash(request.Password),
            email: request.Email
        );

        await _userRepository.AddAsync(newUser, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        return new CreateUserResult(newUser.Id, newUser.Username, newUser.Email);
    }
}