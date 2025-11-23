// ====================================
// Título: ProjectsController
// Descrição: Controller REST API para gerenciar projetos do portfólio
// Autor: Will
// Empresa: WpDev
// Data: 23/11/2024
// ====================================

using MediatR;
using Microsoft.AspNetCore.Mvc;
using Portfolio.Application.Commands.Projects.CreateProject;
using Portfolio.Application.DTOs.Projects;
using Portfolio.Application.Queries.Projects.GetAllProjects;
using Portfolio.Application.Queries.Projects.GetProjectById;
using Portfolio.Application.Commands.Projects.UpdateProject;
using Portfolio.Application.Commands.Projects.DeleteProject;

namespace Portfolio.Api.Controllers;

/// <summary>
/// Controller responsável pelos endpoints de Projects
/// Todos os endpoints usam CQRS via MediatR
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class ProjectsController : ControllerBase
{
    private readonly IMediator _mediator;

    // ====================================
    // CONSTRUTOR - Injeção de Dependência
    // ====================================
    public ProjectsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    // ====================================
    // GET: api/projects
    // ====================================
    /// <summary>
    /// Busca TODOS os projetos ativos
    /// </summary>
    /// <returns>Lista de ProjectCardDto</returns>
    /// <response code="200">Retorna a lista de projetos</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ProjectCardDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<ProjectCardDto>>> GetAll()
    {
        // Cria a query
        var query = new GetAllProjectsQuery();
        
        // Envia para o MediatR processar
        var projects = await _mediator.Send(query);
        
        // Retorna HTTP 200 OK com a lista
        return Ok(projects);
    }

    // ====================================
    // GET: api/projects/{id}
    // ====================================
    /// <summary>
    /// Busca um projeto específico por ID
    /// </summary>
    /// <param name="id">ID do projeto</param>
    /// <returns>ProjectDto completo</returns>
    /// <response code="200">Retorna o projeto encontrado</response>
    /// <response code="404">Projeto não encontrado</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ProjectDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProjectDto>> GetById(Guid id)
    {
        // Cria a query
        var query = new GetProjectByIdQuery(id);
        
        // Envia para o MediatR processar
        var project = await _mediator.Send(query);
        
        // Se não encontrou, retorna 404
        if (project == null)
        {
            return NotFound(new { message = $"Projeto com ID {id} não encontrado" });
        }
        
        // Retorna HTTP 200 OK com o projeto
        return Ok(project);
    }

    // ====================================
    // POST: api/projects
    // ====================================
    /// <summary>
    /// Cria um novo projeto
    /// </summary>
    /// <param name="createDto">Dados do projeto a ser criado</param>
    /// <returns>ProjectDto do projeto criado</returns>
    /// <response code="201">Projeto criado com sucesso</response>
    /// <response code="400">Dados inválidos</response>
    [HttpPost]
    [ProducesResponseType(typeof(ProjectDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ProjectDto>> Create([FromBody] CreateProjectDto createDto)
    {
        // Valida se o DTO veio preenchido
        if (createDto == null)
        {
            return BadRequest(new { message = "Dados do projeto são obrigatórios" });
        }

        // Cria o command
        var command = new CreateProjectCommand(createDto);
        
        // Envia para o MediatR processar
        // O Validator será executado automaticamente aqui
        var createdProject = await _mediator.Send(command);
        
        // Retorna HTTP 201 Created com o projeto criado
        // O header Location apontará para GET /api/projects/{id}
        return CreatedAtAction(
            nameof(GetById), 
            new { id = createdProject.Id }, 
            createdProject
        );
    }

    // ====================================
    // PUT: api/projects/{id}
    // ====================================
    /// <summary>
    /// Atualiza um projeto existente
    /// </summary>
    /// <param name="id">ID do projeto</param>
    /// <param name="updateDto">Dados atualizados</param>
    /// <returns>NoContent se sucesso</returns>
    /// <response code="204">Projeto atualizado com sucesso</response>
    /// <response code="400">Dados inválidos</response>
    /// <response code="404">Projeto não encontrado</response>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateProjectDto updateDto)
    {
        // Valida se o ID da URL bate com o ID do body
        if (id != updateDto.Id)
        {
            return BadRequest(new { message = "O ID da URL não corresponde ao ID do body" });
        }

        try
        {
            // Cria o command
            var command = new UpdateProjectCommand(updateDto);
            
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
    // DELETE: api/projects/{id}
    // ====================================
    /// <summary>
    /// Deleta um projeto (soft delete)
    /// </summary>
    /// <param name="id">ID do projeto</param>
    /// <returns>NoContent se sucesso</returns>
    /// <response code="204">Projeto deletado com sucesso</response>
    /// <response code="404">Projeto não encontrado</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            // Cria o command
            var command = new DeleteProjectCommand(id);
            
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