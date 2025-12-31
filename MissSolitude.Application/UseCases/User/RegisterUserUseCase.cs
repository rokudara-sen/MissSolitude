using MissSolitude.Application.Commands.User;
using MissSolitude.Application.Interfaces.Functions;
using MissSolitude.Application.Interfaces.Repositories;
using MissSolitude.Application.Results.User;

namespace MissSolitude.Application.UseCases.User;

public class RegisterUserUseCase
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IUnitOfWork _unitOfWork;

    public RegisterUserUseCase(IUserRepository userRepository, IPasswordHasher passwordHasher, IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _unitOfWork = unitOfWork;
    }

    public virtual async Task<CreateUserResult> RegisterUserAsync(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        var username = request.Username.Trim();

        if (await _userRepository.EmailExistsAsync(request.Email, cancellationToken))
            throw new InvalidOperationException("Email already in use.");

        if (await _userRepository.UsernameExistsAsync(username, cancellationToken))
            throw new InvalidOperationException("Username already in use.");

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