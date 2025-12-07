// ====================================
// Título: TimelineEventsController.cs
// Descrição: Controller REST API para gerenciar eventos da timeline
// Autor: Will
// Empresa: WpDev
// Data: 29/11/2024
// ====================================

using MediatR;
using Microsoft.AspNetCore.Mvc;
using Portfolio.Application.Commands.TimelineEvents.CreateTimelineEvent;
using Portfolio.Application.Commands.TimelineEvents.UpdateTimelineEvent;
using Portfolio.Application.Commands.TimelineEvents.DeleteTimelineEvent;
using Portfolio.Application.DTOs.TimelineEvents;
using Portfolio.Application.Queries.TimelineEvents.GetAllTimelineEvents;
using Portfolio.Application.Queries.TimelineEvents.GetTimelineEventById;

namespace Portfolio.Api.Controllers;

/// <summary>
/// Controller responsável pelos endpoints de TimelineEvents
/// Todos os endpoints usam CQRS via MediatR
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class TimelineEventsController : ControllerBase
{
    private readonly IMediator _mediator;

    // ====================================
    // CONSTRUTOR - Injeção de Dependência
    // ====================================
    public TimelineEventsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    // ====================================
    // GET: api/timelineevents
    // ====================================
    /// <summary>
    /// Busca TODOS os eventos da timeline
    /// Ordenados por Order (crescente)
    /// </summary>
    /// <returns>Lista de TimelineEventCardDto</returns>
    /// <response code="200">Retorna a lista de eventos</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<TimelineEventCardDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<TimelineEventCardDto>>> GetAll()
    {
        // Cria a query
        var query = new GetAllTimelineEventsQuery();
        
        // Envia para o MediatR processar
        var events = await _mediator.Send(query);
        
        // Retorna HTTP 200 OK com a lista
        return Ok(events);
    }

    // ====================================
    // GET: api/timelineevents/{id}
    // ====================================
    /// <summary>
    /// Busca um evento específico por ID
    /// </summary>
    /// <param name="id">ID do evento</param>
    /// <returns>TimelineEventDto completo</returns>
    /// <response code="200">Retorna o evento encontrado</response>
    /// <response code="404">Evento não encontrado</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(TimelineEventDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TimelineEventDto>> GetById(Guid id)
    {
        // Cria a query
        var query = new GetTimelineEventByIdQuery(id);
        
        // Envia para o MediatR processar
        var timelineEvent = await _mediator.Send(query);
        
        // Se não encontrou, retorna 404
        if (timelineEvent == null)
        {
            return NotFound(new { message = $"Evento com ID {id} não encontrado" });
        }
        
        // Retorna HTTP 200 OK com o evento
        return Ok(timelineEvent);
    }

    // ====================================
    // POST: api/timelineevents
    // ====================================
    /// <summary>
    /// Cria um novo evento
    /// </summary>
    /// <param name="createDto">Dados do evento a ser criado</param>
    /// <returns>TimelineEventDto do evento criado</returns>
    /// <response code="201">Evento criado com sucesso</response>
    /// <response code="400">Dados inválidos</response>
    [HttpPost]
    [ProducesResponseType(typeof(TimelineEventDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<TimelineEventDto>> Create([FromBody] CreateTimelineEventDto createDto)
    {
        // Valida se o DTO veio preenchido
        if (createDto == null)
        {
            return BadRequest(new { message = "Dados do evento são obrigatórios" });
        }

        // Cria o command
        var command = new CreateTimelineEventCommand(createDto);
        
        // Envia para o MediatR processar
        // O Validator será executado automaticamente aqui
        var createdEvent = await _mediator.Send(command);
        
        // Retorna HTTP 201 Created com o evento criado
        // O header Location apontará para GET /api/timelineevents/{id}
        return CreatedAtAction(
            nameof(GetById), 
            new { id = createdEvent.Id }, 
            createdEvent
        );
    }

    // ====================================
    // PUT: api/timelineevents/{id}
    // ====================================
    /// <summary>
    /// Atualiza um evento existente
    /// </summary>
    /// <param name="id">ID do evento</param>
    /// <param name="updateDto">Dados atualizados</param>
    /// <returns>NoContent se sucesso</returns>
    /// <response code="204">Evento atualizado com sucesso</response>
    /// <response code="400">Dados inválidos</response>
    /// <response code="404">Evento não encontrado</response>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateTimelineEventDto updateDto)
    {
        // Valida se o ID da URL bate com o ID do body
        if (id != updateDto.Id)
        {
            return BadRequest(new { message = "O ID da URL não corresponde ao ID do body" });
        }

        try
        {
            // Cria o command
            var command = new UpdateTimelineEventCommand(updateDto);
            
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
    // DELETE: api/timelineevents/{id}
    // ====================================
    /// <summary>
    /// Deleta um evento (SOFT DELETE - marca como invisível)
    /// DIFERENTE do BlogPost que usa hard delete
    /// O evento pode ser recuperado depois
    /// </summary>
    /// <param name="id">ID do evento</param>
    /// <returns>NoContent se sucesso</returns>
    /// <response code="204">Evento deletado com sucesso</response>
    /// <response code="404">Evento não encontrado</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            // Cria o command
            var command = new DeleteTimelineEventCommand(id);
            
            // Envia para o MediatR processar
            await _mediator.Send(command);
            
            // Retorna HTTP 204 No Content (sucesso)
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }
}