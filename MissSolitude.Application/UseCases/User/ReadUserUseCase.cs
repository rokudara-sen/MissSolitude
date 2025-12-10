using MissSolitude.Application.Interfaces.Repositories;
using MissSolitude.Application.Results;

namespace MissSolitude.Application.UseCases.User;

public class ReadUserUseCase
{
    private readonly IUserRepository _userRepository;
    
    public ReadUserUseCase(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<ReadUserResult> ExecuteAsync(Guid id, CancellationToken cancellationToken)
    {
        var existingUser = await _userRepository.GetByIdAsync(id, cancellationToken);
        
        if(existingUser is null)
            throw new KeyNotFoundException("User not found.");
        
        return new ReadUserResult(existingUser.Id, existingUser.Username, existingUser.Email);
    }
}