// ====================================
// Título: ContactMessagesController.cs
// Descrição: Controller REST API para gerenciar mensagens de contato
// Autor: Will
// Empresa: WpDev
// Data: 06/12/2024
// ====================================

using MediatR;
using Microsoft.AspNetCore.Mvc;
using Portfolio.Application.Commands.ContactMessages.CreateContactMessage;
using Portfolio.Application.Commands.ContactMessages.UpdateContactMessage;
using Portfolio.Application.DTOs.ContactMessages;
using Portfolio.Application.Queries.ContactMessages.GetAllContactMessages;
using Portfolio.Application.Queries.ContactMessages.GetContactMessageById;

namespace Portfolio.Api.Controllers;

/// <summary>
/// Controller responsável pelos endpoints de ContactMessages
/// Todos os endpoints usam CQRS via MediatR
/// POST é público (formulário de contato)
/// GET e PUT são ADMIN (gerenciamento de mensagens)
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class ContactMessagesController : ControllerBase
{
    private readonly IMediator _mediator;

    // ====================================
    // CONSTRUTOR - Injeção de Dependência
    // ====================================
    public ContactMessagesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    // ====================================
    // GET: api/contactmessages
    // ====================================
    /// <summary>
    /// Busca TODAS as mensagens de contato (ADMIN)
    /// Ordenadas por CreatedAt (mais recentes primeiro)
    /// </summary>
    /// <returns>Lista de ContactMessageCardDto</returns>
    /// <response code="200">Retorna a lista de mensagens</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ContactMessageCardDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<ContactMessageCardDto>>> GetAll()
    {
        // Cria a query
        var query = new GetAllContactMessagesQuery();
        
        // Envia para o MediatR processar
        var messages = await _mediator.Send(query);
        
        // Retorna HTTP 200 OK com a lista
        return Ok(messages);
    }

    // ====================================
    // GET: api/contactmessages/{id}
    // ====================================
    /// <summary>
    /// Busca uma mensagem específica por ID (ADMIN)
    /// </summary>
    /// <param name="id">ID da mensagem</param>
    /// <returns>ContactMessageDto completo</returns>
    /// <response code="200">Retorna a mensagem encontrada</response>
    /// <response code="404">Mensagem não encontrada</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ContactMessageDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ContactMessageDto>> GetById(Guid id)
    {
        // Cria a query
        var query = new GetContactMessageByIdQuery(id);
        
        // Envia para o MediatR processar
        var contactMessage = await _mediator.Send(query);
        
        // Se não encontrou, retorna 404
        if (contactMessage == null)
        {
            return NotFound(new { message = $"Mensagem com ID {id} não encontrada" });
        }
        
        // Retorna HTTP 200 OK com a mensagem
        return Ok(contactMessage);
    }

    // ====================================
    // POST: api/contactmessages
    // ====================================
    /// <summary>
    /// Cria uma nova mensagem de contato (PÚBLICO - Formulário)
    /// Captura automaticamente IP e User Agent para segurança
    /// </summary>
    /// <param name="createDto">Dados da mensagem a ser criada</param>
    /// <returns>ContactMessageDto da mensagem criada</returns>
    /// <response code="201">Mensagem criada com sucesso</response>
    /// <response code="400">Dados inválidos</response>
    [HttpPost]
    [ProducesResponseType(typeof(ContactMessageDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ContactMessageDto>> Create([FromBody] CreateContactMessageDto createDto)
    {
        // Valida se o DTO veio preenchido
        if (createDto == null)
        {
            return BadRequest(new { message = "Dados da mensagem são obrigatórios" });
        }

        // Cria o command
        var command = new CreateContactMessageCommand(createDto);
        
        // ====================================
        // CAPTURA IP E USER AGENT (segurança)
        // ====================================
        command.IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
        command.UserAgent = HttpContext.Request.Headers["User-Agent"].ToString();
        
        // Envia para o MediatR processar
        // O Validator será executado automaticamente aqui
        var createdMessage = await _mediator.Send(command);
        
        // Retorna HTTP 201 Created com a mensagem criada
        // O header Location apontará para GET /api/contactmessages/{id}
        return CreatedAtAction(
            nameof(GetById), 
            new { id = createdMessage.Id }, 
            createdMessage
        );
    }

    // ====================================
    // PUT: api/contactmessages/{id}
    // ====================================
    /// <summary>
    /// Atualiza uma mensagem existente (ADMIN)
    /// Usado para mudar status, adicionar resposta, marcar como spam
    /// </summary>
    /// <param name="id">ID da mensagem</param>
    /// <param name="updateDto">Dados atualizados</param>
    /// <returns>NoContent se sucesso</returns>
    /// <response code="204">Mensagem atualizada com sucesso</response>
    /// <response code="400">Dados inválidos</response>
    /// <response code="404">Mensagem não encontrada</response>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateContactMessageDto updateDto)
    {
        // Valida se o ID da URL bate com o ID do body
        if (id != updateDto.Id)
        {
            return BadRequest(new { message = "O ID da URL não corresponde ao ID do body" });
        }

        try
        {
            // Cria o command
            var command = new UpdateContactMessageCommand(updateDto);
            
            // Envia para o MediatR processar
            await _mediator.Send(command);
            
            // Retorna HTTP 204 No Content (sucesso, sem corpo)
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    // ====================================
    // NOTA: SEM ENDPOINT DELETE
    // ====================================
    // ContactMessage NÃO tem endpoint DELETE
    // Todas mensagens são mantidas para auditoria
    // Use PUT para mudar Status = Archived ou Spam
}