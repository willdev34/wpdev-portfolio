// ====================================
// Título: NowSectionsController.cs
// Descrição: Controller REST API para gerenciar a seção "O que estou fazendo agora"
// Autor: Will
// Empresa: WpDev
// Data: 06/12/2024
// ====================================

using MediatR;
using Microsoft.AspNetCore.Mvc;
using Portfolio.Application.Commands.NowSections.CreateNowSection;
using Portfolio.Application.Commands.NowSections.UpdateNowSection;
using Portfolio.Application.Commands.NowSections.DeleteNowSection;
using Portfolio.Application.DTOs.NowSections;
using Portfolio.Application.Queries.NowSections.GetAllNowSections;
using Portfolio.Application.Queries.NowSections.GetNowSectionById;
using Portfolio.Application.Queries.NowSections.GetActiveNowSection;

namespace Portfolio.Api.Controllers;

/// <summary>
/// Controller responsável pelos endpoints de NowSection
/// Todos os endpoints usam CQRS via MediatR
/// GET /api/nowsection (sem 's') é público - busca seção ativa
/// Outros endpoints são ADMIN
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class NowSectionsController : ControllerBase
{
    private readonly IMediator _mediator;

    // ====================================
    // CONSTRUTOR - Injeção de Dependência
    // ====================================
    public NowSectionsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    // ====================================
    // GET: api/nowsection (SEM 'S') - PÚBLICO
    // ====================================
    /// <summary>
    /// Busca a seção Now ATIVA (IsActive = true) - PÚBLICO
    /// Usado para exibir no site público
    /// IMPORTANTE: Rota sem 's' no final
    /// </summary>
    /// <returns>NowSectionDto da seção ativa ou null</returns>
    /// <response code="200">Retorna a seção ativa</response>
    /// <response code="404">Nenhuma seção ativa encontrada</response>
    [HttpGet("/api/nowsection")]
    [ProducesResponseType(typeof(NowSectionDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<NowSectionDto>> GetActive()
    {
        // Cria a query
        var query = new GetActiveNowSectionQuery();
        
        // Envia para o MediatR processar
        var nowSection = await _mediator.Send(query);
        
        // Se não encontrou, retorna 404
        if (nowSection == null)
        {
            return NotFound(new { message = "Nenhuma seção Now ativa encontrada" });
        }
        
        // Retorna HTTP 200 OK com a seção
        return Ok(nowSection);
    }

    // ====================================
    // GET: api/nowsections (COM 'S') - ADMIN
    // ====================================
    /// <summary>
    /// Busca TODAS as seções Now (ADMIN)
    /// Ordenadas por UpdatedAt (mais recente primeiro)
    /// </summary>
    /// <returns>Lista de NowSectionDto</returns>
    /// <response code="200">Retorna a lista de seções</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<NowSectionDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<NowSectionDto>>> GetAll()
    {
        // Cria a query
        var query = new GetAllNowSectionsQuery();
        
        // Envia para o MediatR processar
        var sections = await _mediator.Send(query);
        
        // Retorna HTTP 200 OK com a lista
        return Ok(sections);
    }

    // ====================================
    // GET: api/nowsections/{id} - ADMIN
    // ====================================
    /// <summary>
    /// Busca uma seção específica por ID (ADMIN)
    /// </summary>
    /// <param name="id">ID da seção</param>
    /// <returns>NowSectionDto completo</returns>
    /// <response code="200">Retorna a seção encontrada</response>
    /// <response code="404">Seção não encontrada</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(NowSectionDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<NowSectionDto>> GetById(Guid id)
    {
        // Cria a query
        var query = new GetNowSectionByIdQuery(id);
        
        // Envia para o MediatR processar
        var nowSection = await _mediator.Send(query);
        
        // Se não encontrou, retorna 404
        if (nowSection == null)
        {
            return NotFound(new { message = $"Seção com ID {id} não encontrada" });
        }
        
        // Retorna HTTP 200 OK com a seção
        return Ok(nowSection);
    }

    // ====================================
    // POST: api/nowsections
    // ====================================
    /// <summary>
    /// Cria uma nova seção Now
    /// IMPORTANTE: Desativa automaticamente todas as seções anteriores
    /// Apenas 1 seção pode estar ativa por vez (business rule)
    /// </summary>
    /// <param name="createDto">Dados da seção a ser criada</param>
    /// <returns>NowSectionDto da seção criada</returns>
    /// <response code="201">Seção criada com sucesso</response>
    /// <response code="400">Dados inválidos</response>
    [HttpPost]
    [ProducesResponseType(typeof(NowSectionDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<NowSectionDto>> Create([FromBody] CreateNowSectionDto createDto)
    {
        // Valida se o DTO veio preenchido
        if (createDto == null)
        {
            return BadRequest(new { message = "Dados da seção são obrigatórios" });
        }

        // Cria o command
        var command = new CreateNowSectionCommand(createDto);
        
        // Envia para o MediatR processar
        // O Validator será executado automaticamente aqui
        var createdSection = await _mediator.Send(command);
        
        // Retorna HTTP 201 Created com a seção criada
        // O header Location apontará para GET /api/nowsections/{id}
        return CreatedAtAction(
            nameof(GetById), 
            new { id = createdSection.Id }, 
            createdSection
        );
    }

    // ====================================
    // PUT: api/nowsections/{id}
    // ====================================
    /// <summary>
    /// Atualiza uma seção existente
    /// IMPORTANTE: Se mudar IsActive para true, desativa todas as outras automaticamente
    /// </summary>
    /// <param name="id">ID da seção</param>
    /// <param name="updateDto">Dados atualizados</param>
    /// <returns>NoContent se sucesso</returns>
    /// <response code="204">Seção atualizada com sucesso</response>
    /// <response code="400">Dados inválidos</response>
    /// <response code="404">Seção não encontrada</response>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateNowSectionDto updateDto)
    {
        // Valida se o ID da URL bate com o ID do body
        if (id != updateDto.Id)
        {
            return BadRequest(new { message = "O ID da URL não corresponde ao ID do body" });
        }

        try
        {
            // Cria o command
            var command = new UpdateNowSectionCommand(updateDto);
            
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
    // DELETE: api/nowsections/{id}
    // ====================================
    /// <summary>
    /// Deleta uma seção (SOFT DELETE - marca como inativa)
    /// A seção pode ser recuperada depois mudando IsActive para true
    /// </summary>
    /// <param name="id">ID da seção</param>
    /// <returns>NoContent se sucesso</returns>
    /// <response code="204">Seção deletada com sucesso</response>
    /// <response code="404">Seção não encontrada</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            // Cria o command
            var command = new DeleteNowSectionCommand(id);
            
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