using Microsoft.AspNetCore.Mvc;
using MissSolitude.Application.Commands;
using MissSolitude.Application.Commands.User;
using MissSolitude.Application.UseCases.User;
using MissSolitude.Contracts;
using MissSolitude.Contracts.User;

namespace MissSolitude.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly CreateUserUseCase _createUserUseCase;
    private readonly ReadUserUseCase _readUserUseCase;
    private readonly UpdateUserUseCase _updateUserUseCase;
    private readonly DeleteUserUseCase _deleteUserUseCase;
    private readonly LogInUserUseCase _logInUserUseCase;

    public UserController(CreateUserUseCase createUserUseCase, ReadUserUseCase readUserUseCase, UpdateUserUseCase updateUserUseCase, DeleteUserUseCase deleteUserUseCase, LogInUserUseCase logInUserUseCase)
    {
        _createUserUseCase = createUserUseCase;
        _readUserUseCase = readUserUseCase;
        _updateUserUseCase = updateUserUseCase;
        _deleteUserUseCase = deleteUserUseCase;
        _logInUserUseCase = logInUserUseCase;
    }

    [HttpPost]
    public async Task<IActionResult> CreateUserAsync([FromBody] CreateUserCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await _createUserUseCase.ExecuteAsync(request, cancellationToken);
            var userDto = new UserDto(result.Id, result.Username, result.Email);
            return CreatedAtRoute("GetUserById", new { id = userDto.Id }, userDto);
        }
        catch (InvalidOperationException exception)
        {
            return BadRequest(exception.Message);
        }
    }

    [HttpGet("{id:guid}", Name = "GetUserById")]
    public async Task<IActionResult> ReadUserAsync(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _readUserUseCase.ExecuteAsync(id, cancellationToken);
            var userDto = new UserDto(result.Id, result.Username, result.Email);
            return Ok(userDto);
        }
        catch (KeyNotFoundException exception)
        {
            return NotFound(exception.Message);
        }
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateUserAsync(Guid id, [FromBody] UpdateUserCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            if (id != request.Id)
                return BadRequest("Body id and route id do not match.");

            var result = await _updateUserUseCase.ExecuteAsync(request, cancellationToken);
            var userDto = new UserDto(result.Id, result.Username, result.Email);
            return Ok(userDto);
        }
        catch (KeyNotFoundException exception)
        {
            return NotFound(exception.Message);
        }
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteUserAsync(Guid id,
        CancellationToken cancellationToken)
    {
        try
        {
            await _deleteUserUseCase.ExecuteAsync(id, cancellationToken);
            return NoContent();
        }
        catch (KeyNotFoundException exception)
        {
            return NotFound(exception.Message);
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> LoginUserAsync([FromBody] LogInUserCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await _logInUserUseCase.LogInAsync(request, cancellationToken);
            return Ok(result);
        }
        catch (UnauthorizedAccessException exception)
        {
            return Unauthorized(exception.Message);
        }
    }
}