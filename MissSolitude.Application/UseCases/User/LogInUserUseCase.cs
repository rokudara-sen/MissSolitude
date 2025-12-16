using MissSolitude.Application.Commands;
using MissSolitude.Application.Commands.User;
using MissSolitude.Application.Interfaces.Functions;
using MissSolitude.Application.Interfaces.Repositories;
using MissSolitude.Application.Results;
using MissSolitude.Application.Results.User;

namespace MissSolitude.Application.UseCases.User;

public class LogInUserUseCase
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenService _tokenService;
    
    public LogInUserUseCase(IUserRepository userRepository, IPasswordHasher passwordHasher, ITokenService tokenService)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _tokenService = tokenService;
    }
    
    public virtual async Task<LogInUserResult> LogInAsync(LogInUserCommand request, CancellationToken cancellationToken)
    {
        var existingUser = await _userRepository.GetByEmailOrUsernameAsync(request.Identifier, cancellationToken);
        
        if(existingUser is null || !_passwordHasher.Verify(request.Password, existingUser.PasswordHash))
        {
            throw new UnauthorizedAccessException("Invalid credentials.");
        }
        var tokenUser = new TokenUser(existingUser.Id, existingUser.Username, existingUser.Email);
        var tokens = _tokenService.IssueTokens(tokenUser);
            

        return new LogInUserResult(tokens.AccessToken, tokens.RefreshToken, new ReadUserResult(existingUser.Id, existingUser.Username, existingUser.Email));
    }
}