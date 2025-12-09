using Microsoft.AspNetCore.Mvc;
using MissSolitude.Application.Commands;
using MissSolitude.Application.Results;
using MissSolitude.Application.UseCases.User;

namespace MissSolitude.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly CreateUserUseCase _createUserUseCase;
    private readonly ReadUserUseCase _readUserUseCase;
    private readonly UpdateUserUseCase _updateUserUseCase;
    private readonly DeleteUserUseCase _deleteUserUseCase;

    public UserController(CreateUserUseCase createUserUseCase, ReadUserUseCase readUserUseCase, UpdateUserUseCase updateUserUseCase, DeleteUserUseCase deleteUserUseCase)
    {
        _createUserUseCase = createUserUseCase;
        _readUserUseCase = readUserUseCase;
        _updateUserUseCase = updateUserUseCase;
        _deleteUserUseCase = deleteUserUseCase;
    }

    [HttpPost]
    public Task<CreateUserResult> CreateUserAsync([FromBody] CreateUserCommand request,
        CancellationToken cancellationToken)
    {
        return _createUserUseCase.ExecuteAsync(request, cancellationToken);
    }
    
    [HttpGet("{id:guid}")]
    public Task<ReadUserResult> ReadUserAsync(Guid id, CancellationToken cancellationToken)
    {
        return _readUserUseCase.ExecuteAsync(id, cancellationToken);
    }

    [HttpPut]
    public Task<UpdateUserResult> UpdateUserAsync([FromBody] UpdateUserCommand request,
        CancellationToken cancellationToken)
    {
        return _updateUserUseCase.ExecuteAsync(request, cancellationToken);
    }

    [HttpDelete("{id:guid}")]
    public Task DeleteUserAsync(Guid id,
        CancellationToken cancellationToken)
    {
        return _deleteUserUseCase.ExecuteAsync(id, cancellationToken);
    }
}