using Microsoft.AspNetCore.Mvc;
using MissSolitude.Contracts;
using MissSolitude.Domain;
using MissSolitude.Infrastructure;

namespace MissSolitude.API;

[Route("api/[controller]")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly DatabaseContext _databaseContext;

    public UserController(DatabaseContext databaseContext)
    {
        _databaseContext = databaseContext;
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
        if (user is null) return NotFound();

        return Ok(new UserDto(user.Id, user.Username, user.Email));
    }
    
    [HttpPost]
    public IActionResult AddUser(CreateUserRequest user)
    {
        var userExistsAlready =
            _databaseContext.Users.Any(databaseContextUser => databaseContextUser.Email == user.Email);
        if (userExistsAlready)
            return Conflict("User already exists.");

        var newUser = new User(Guid.NewGuid(), user.Username, user.Password, user.Email);

        _databaseContext.Users.Add(newUser);
        _databaseContext.SaveChanges();

        var dataTransferObject = new UserDto(newUser.Id, newUser.Username, newUser.Email);

        return CreatedAtAction(nameof(GetById), new { id = newUser.Id }, dataTransferObject);
    }
}