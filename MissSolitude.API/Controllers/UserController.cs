using System.Data;
using Microsoft.AspNetCore.Mvc;
using MissSolitude.Application;
using MissSolitude.Application.Commands;
using MissSolitude.Application.Services;
using MissSolitude.Application.Services.Interfaces;
using MissSolitude.Contracts;
using MissSolitude.Infrastructure;

namespace MissSolitude.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly DatabaseContext _databaseContext;
    private readonly IUserService _userService;

    public UserController(DatabaseContext databaseContext, IUserService userService)
    {
        _databaseContext = databaseContext;
        _userService = userService;
    }

    [HttpGet]
    public IActionResult GetAll()
    {
        return Ok(_databaseContext.Users.Select(user => new UserDto(user.Id, user.Username, user.Email)));
    }

    [HttpGet("{id:guid}")]
    public IActionResult GetById(Guid id)
    {
        var user = _databaseContext.Users.Find(id);
        if (user is null)
            return NotFound();

        return Ok(new UserDto(user.Id, user.Username, user.Email));
    }
    
    [HttpPost]
    public async Task<IActionResult> AddUserAsync([FromBody] CreateUserRequest user, CancellationToken cancellationToken)
    {
        try
        {
            var command = new CreateUserCommand(
                Username: user.Username,
                Email: user.Email,
                Password: user.Password
            );
            
            var result = await _userService.CreateAsync(command, cancellationToken);
            
            var dataTransferObject = new UserDto(result.Id, result.Username, result.Email);
            
            return CreatedAtAction(nameof(GetById), new { id = dataTransferObject.Id }, dataTransferObject);
        }
        catch (DuplicateNameException exception)
        {
            return Conflict(exception.Message);
        }
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> RemoveUserAsync(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var command = new RemoveUserCommand(id);

            await _userService.RemoveAsync(command, cancellationToken);

            return NoContent();
        }
        catch (KeyNotFoundException exception)
        {
            return NotFound(exception.Message);
        }
    }
}