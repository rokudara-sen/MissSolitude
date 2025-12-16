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
    private readonly UpdateContactUseCase _updateContactUseCase;
    private readonly DeleteContactUseCase _deleteContactUseCase;
    
    public ContactController(CreateContactUseCase createContactUseCase, ReadContactUseCase readContactUseCase, UpdateContactUseCase updateContactUseCase, DeleteContactUseCase deleteContactUseCase)
    {
        _createContactUseCase = createContactUseCase;
        _readContactUseCase = readContactUseCase;
        _updateContactUseCase = updateContactUseCase;
        _deleteContactUseCase = deleteContactUseCase;
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

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateContactAsync(Guid id, [FromBody] UpdateContactCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            if (id != request.Id) 
                return BadRequest("Body id and route id do not match.");
            
            var result = await _updateContactUseCase.ExecuteAsync(request, cancellationToken);
            var contactDto = new ContactDto(result.Id, result.FirstName, result.LastName, result.Email, result.Phone, result.Notes);
            return Ok(contactDto);
        }
        catch (KeyNotFoundException exception)
        {
            return NotFound(exception.Message);
        }
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteContact(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            await _deleteContactUseCase.ExecuteAsync(id, cancellationToken);
            return NoContent();
        }
        catch (KeyNotFoundException exception)
        {
            return NotFound(exception.Message);
        }
    }
}