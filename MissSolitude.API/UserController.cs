using Microsoft.AspNetCore.Mvc;
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
        var listOfUsers = _databaseContext.Users.ToList();
        return Ok(listOfUsers);
    }
}