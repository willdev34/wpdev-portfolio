// Título: ContactMessagesController.cs
// Descrição: Controller REST API para gerenciar mensagens de contato

using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Portfolio.Application.Commands.ContactMessages.CreateContactMessage;
using Portfolio.Application.Commands.ContactMessages.DeleteContactMessage;
using Portfolio.Application.Commands.ContactMessages.UpdateContactMessage;
using Portfolio.Application.DTOs.ContactMessages;
using Portfolio.Application.Queries.ContactMessages.GetAllContactMessages;
using Portfolio.Application.Queries.ContactMessages.GetContactMessageById;

namespace Portfolio.Api.Controllers;

/// <summary>
/// Controller responsável pelos endpoints de ContactMessages
/// POST é público (formulário de contato)
/// GET, PUT e DELETE são ADMIN (requerem autenticação JWT)
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class ContactMessagesController : ControllerBase
{
    private readonly IMediator _mediator;

    public ContactMessagesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    // GET: api/contactmessages (ADMIN)
    [HttpGet]
    [Authorize]
    [ProducesResponseType(typeof(IEnumerable<ContactMessageCardDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<ContactMessageCardDto>>> GetAll()
    {
        var query = new GetAllContactMessagesQuery();
        var messages = await _mediator.Send(query);
        return Ok(messages);
    }

    // GET: api/contactmessages/{id} (ADMIN)
    [HttpGet("{id}")]
    [Authorize]
    [ProducesResponseType(typeof(ContactMessageDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ContactMessageDto>> GetById(Guid id)
    {
        var query = new GetContactMessageByIdQuery(id);
        var contactMessage = await _mediator.Send(query);

        if (contactMessage == null)
        {
            return NotFound(new { message = $"Mensagem com ID {id} não encontrada" });
        }

        return Ok(contactMessage);
    }

    // POST: api/contactmessages (PÚBLICO - formulário)
    [HttpPost]
    [ProducesResponseType(typeof(ContactMessageDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ContactMessageDto>> Create([FromBody] CreateContactMessageDto createDto)
    {
        if (createDto == null)
        {
            return BadRequest(new { message = "Dados da mensagem são obrigatórios" });
        }

        var command = new CreateContactMessageCommand(createDto);
        command.IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
        command.UserAgent = HttpContext.Request.Headers["User-Agent"].ToString();

        var createdMessage = await _mediator.Send(command);

        return CreatedAtAction(
            nameof(GetById),
            new { id = createdMessage.Id },
            createdMessage
        );
    }

    // PUT: api/contactmessages/{id} (ADMIN)
    [HttpPut("{id}")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateContactMessageDto updateDto)
    {
        if (id != updateDto.Id)
        {
            return BadRequest(new { message = "O ID da URL não corresponde ao ID do body" });
        }

        try
        {
            var command = new UpdateContactMessageCommand(updateDto);
            await _mediator.Send(command);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    // DELETE: api/contactmessages/{id} (ADMIN)
    [HttpDelete("{id}")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            var command = new DeleteContactMessageCommand(id);
            await _mediator.Send(command);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }
}