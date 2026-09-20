// ====================================
// Título: ProjectsController
// Descrição: Controller REST API para gerenciar projetos do portfólio
// ====================================

using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Portfolio.Application.Commands.Projects.CreateProject;
using Portfolio.Application.Commands.Projects.UpdateProject;
using Portfolio.Application.Commands.Projects.DeleteProject;
using Portfolio.Application.DTOs.Projects;
using Portfolio.Application.Queries.Projects.GetAllProjects;
using Portfolio.Application.Queries.Projects.GetFeaturedProjects;
using Portfolio.Application.Queries.Projects.GetProjectById;

namespace Portfolio.Api.Controllers;

/// <summary>
/// Controller responsável pelos endpoints de Projects
/// Todos os endpoints usam CQRS via MediatR
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
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
    [AllowAnonymous]
    [ProducesResponseType(typeof(IEnumerable<ProjectCardDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<ProjectCardDto>>> GetAll()
    {
        var query = new GetAllProjectsQuery();
        var projects = await _mediator.Send(query);
        return Ok(projects);
    }

    // ====================================
    // GET: api/projects/featured
    // ====================================
    /// <summary>
    /// Busca apenas projetos em destaque (IsFeatured = true)
    /// Resultado cacheado por 5 minutos no browser
    /// </summary>
    /// <returns>Lista de ProjectCardDto em destaque</returns>
    /// <response code="200">Retorna os projetos em destaque</response>
    [HttpGet("featured")]
    [AllowAnonymous]
    [ResponseCache(Duration = 300)]
    [ProducesResponseType(typeof(IEnumerable<ProjectCardDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<ProjectCardDto>>> GetFeatured()
    {
        var query = new GetFeaturedProjectsQuery();
        var projects = await _mediator.Send(query);
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
    [AllowAnonymous]
    [ProducesResponseType(typeof(ProjectDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProjectDto>> GetById(Guid id)
    {
        var query = new GetProjectByIdQuery(id);
        var project = await _mediator.Send(query);

        if (project == null)
        {
            return NotFound(new { message = $"Projeto com ID {id} não encontrado" });
        }

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
        if (createDto == null)
        {
            return BadRequest(new { message = "Dados do projeto são obrigatórios" });
        }

        var command = new CreateProjectCommand(createDto);
        var createdProject = await _mediator.Send(command);

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
        if (id != updateDto.Id)
        {
            return BadRequest(new { message = "O ID da URL não corresponde ao ID do body" });
        }

        try
        {
            var command = new UpdateProjectCommand(updateDto);
            await _mediator.Send(command);
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
            var command = new DeleteProjectCommand(id);
            await _mediator.Send(command);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }
}