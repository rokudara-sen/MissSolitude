using System.Data;
using Microsoft.AspNetCore.Mvc;
using MissSolitude.Application;
using MissSolitude.Contracts;
using MissSolitude.Infrastructure;

namespace MissSolitude.API;

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
    public async Task<IActionResult> AddUser([FromBody] CreateUserRequest user)
    {
        try
        {
            var command = new CreateUserCommand(
                Username: user.Username,
                Email: user.Email,
                Password: user.Password
            );
            
            var result = await _userService.CreateAsync(command);
            
            var dataTransferObject = new UserDto(result.Id, result.Username, result.Email);
            
            return CreatedAtAction(nameof(GetById), new { id = dataTransferObject.Id }, dataTransferObject);
        }
        catch (DuplicateNameException exception)
        {
            return Conflict(exception.Message);
        }
    }

    [HttpDelete("{id:guid}")]
    public IActionResult RemoveUser(Guid id)
    {
        var user = _databaseContext.Users.Find(id);
        if (user is null)
            return NotFound();

        _databaseContext.Users.Remove(user);
        _databaseContext.SaveChanges();
        
        return NoContent();
    }
}