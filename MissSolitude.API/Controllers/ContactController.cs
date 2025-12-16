using Microsoft.AspNetCore.Mvc;
using MissSolitude.Application.Commands;
using MissSolitude.Application.Commands.Contact;
using MissSolitude.Application.UseCases.Contact;
using MissSolitude.Contracts.Contact;

namespace MissSolitude.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ContactController : ControllerBase
{
    private readonly CreateContactUseCase _createContactUseCase;
    private readonly ReadContactUseCase _readContactUseCase;
    
    public ContactController(CreateContactUseCase createContactUseCase, ReadContactUseCase readContactUseCase)
    {
        _createContactUseCase = createContactUseCase;
        _readContactUseCase = readContactUseCase;
    }
    
    [HttpPost]
    public async Task<IActionResult> CreateContactAsync([FromBody] CreateContactCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await _createContactUseCase.ExecuteAsync(request, cancellationToken);
            var contactDto = new ContactDto(result.Id, result.FirstName, result.LastName, result.Email, result.Phone, result.Notes);
            return CreatedAtRoute("GetContactById", new { id = contactDto.Id }, contactDto);
        }
        catch (InvalidOperationException exception)
        {
            return BadRequest(exception.Message);
        }
    }
    
    [HttpGet("{id:guid}", Name = "GetContactById")]
    public async Task<IActionResult> ReadContactAsync(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _readContactUseCase.ExecuteAsync(id, cancellationToken);
            var contactDto = new ContactDto(result.Id, result.FirstName, result.LastName, result.Email, result.Phone, result.Notes);
            return Ok(contactDto);
        }
        catch (KeyNotFoundException exception)
        {
            return NotFound(exception.Message);
        }
    }
}