// ====================================
// Título: BlogPostsController.cs
// Descrição: Controller REST API para gerenciar posts do blog
// ====================================

using MediatR;
using Microsoft.AspNetCore.Mvc;
using Portfolio.Application.Commands.BlogPosts.CreateBlogPost;
using Portfolio.Application.Commands.BlogPosts.UpdateBlogPost;
using Portfolio.Application.Commands.BlogPosts.BulkDeleteBlogPosts;
using Portfolio.Application.Commands.BlogPosts.DeleteBlogPost;
using Portfolio.Application.DTOs.BlogPosts;
using Portfolio.Application.Queries.BlogPosts.GetAllBlogPosts;
using Portfolio.Application.Queries.BlogPosts.GetBlogPostById;
using Portfolio.Application.Queries.BlogPosts.GetBlogPostBySlug;
using Portfolio.Application.Queries.BlogPosts.GetPublicBlogPostById;
using Portfolio.Application.Queries.BlogPosts.GetPublicBlogPosts;

namespace Portfolio.Api.Controllers;

/// <summary>
/// Controller responsável pelos endpoints de BlogPosts
/// Todos os endpoints usam CQRS via MediatR
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Microsoft.AspNetCore.Authorization.Authorize]
public class BlogPostsController : ControllerBase
{
    private readonly IMediator _mediator;

    // ====================================
    // CONSTRUTOR - Injeção de Dependência
    // ====================================
    public BlogPostsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    // ====================================
    // GET: api/blogposts/public
    // ====================================
    /// <summary>
    /// Busca posts PUBLICADOS para exibição pública
    /// Filtra apenas posts com IsPublished = true
    /// </summary>
    /// <returns>Lista de BlogPostCardDto publicados</returns>
    /// <response code="200">Retorna a lista de posts publicados</response>
    [HttpGet("public")]
    [Microsoft.AspNetCore.Authorization.AllowAnonymous]
    [ProducesResponseType(typeof(IEnumerable<BlogPostCardDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<BlogPostCardDto>>> GetPublic()
    {
        // Cria a query para posts públicos (IsPublished = true)
        var query = new GetPublicBlogPostsQuery();

        // Envia para o MediatR processar
        var posts = await _mediator.Send(query);

        // Retorna HTTP 200 OK com a lista (apenas publicados)
        return Ok(posts);
    }

    // ====================================
    // GET: api/blogposts
    // ====================================
    /// <summary>
    /// Busca TODOS os posts (publicados e rascunhos) - ADMIN ONLY
    /// </summary>
    /// <returns>Lista de BlogPostAdminCardDto (inclui Status e ScheduledAt)</returns>
    /// <response code="200">Retorna a lista de posts</response>
    /// <response code="401">Não autorizado</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<BlogPostAdminCardDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<IEnumerable<BlogPostAdminCardDto>>> GetAll()
    {
        // Cria a query (retorna TODOS, incluindo rascunhos)
        var query = new GetAllBlogPostsQuery();

        // Envia para o MediatR processar
        var posts = await _mediator.Send(query);

        // Retorna HTTP 200 OK com a lista
        return Ok(posts);
    }

    // ====================================
    // GET: api/blogposts/{id}
    // ====================================
    /// <summary>
    /// Busca um post específico por ID para exibição PÚBLICA
    /// Aplica a mesma regra de visibilidade da listagem pública
    /// (Published, ou Scheduled com ScheduledAt já vencido)
    /// </summary>
    /// <param name="id">ID do post</param>
    /// <returns>BlogPostDto completo</returns>
    /// <response code="200">Retorna o post encontrado</response>
    /// <response code="404">Post não encontrado ou não público</response>
    [HttpGet("{id}")]
    [Microsoft.AspNetCore.Authorization.AllowAnonymous]
    [ProducesResponseType(typeof(BlogPostDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<BlogPostDto>> GetById(Guid id)
    {
        // Cria a query pública (filtra por visibilidade)
        var query = new GetPublicBlogPostByIdQuery(id);

        // Envia para o MediatR processar
        var post = await _mediator.Send(query);

        // Se não encontrou ou não é público, retorna 404
        if (post == null)
        {
            return NotFound(new { message = $"Post com ID {id} não encontrado" });
        }

        // Retorna HTTP 200 OK com o post
        return Ok(post);
    }

    // ====================================
    // GET: api/blogposts/admin/{id}
    // ====================================
    /// <summary>
    /// Busca um post específico por ID para o ADMIN (tela de edição)
    /// Não aplica filtro de visibilidade: retorna Draft, Scheduled e Published
    /// </summary>
    /// <param name="id">ID do post</param>
    /// <returns>BlogPostDto completo</returns>
    /// <response code="200">Retorna o post encontrado</response>
    /// <response code="401">Não autorizado</response>
    /// <response code="404">Post não encontrado</response>
    [HttpGet("admin/{id}")]
    [ProducesResponseType(typeof(BlogPostDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<BlogPostDto>> GetByIdAdmin(Guid id)
    {
        // Cria a query sem filtro de visibilidade
        var query = new GetBlogPostByIdQuery(id);

        // Envia para o MediatR processar
        var post = await _mediator.Send(query);

        // Se não encontrou, retorna 404
        if (post == null)
        {
            return NotFound(new { message = $"Post com ID {id} não encontrado" });
        }

        // Retorna HTTP 200 OK com o post
        return Ok(post);
    }

    // ====================================
    // GET: api/blogposts/slug/{slug}
    // ====================================
    /// <summary>
    /// Busca um post por Slug (URL amigável)
    /// Exemplo: /api/blogposts/slug/como-aprender-dotnet
    /// BÔNUS: Incrementa o contador de visualizações automaticamente
    /// </summary>
    /// <param name="slug">Slug do post</param>
    /// <returns>BlogPostDto completo</returns>
    /// <response code="200">Retorna o post encontrado</response>
    /// <response code="404">Post não encontrado</response>
    [HttpGet("slug/{slug}")]
    [Microsoft.AspNetCore.Authorization.AllowAnonymous]
    [ProducesResponseType(typeof(BlogPostDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<BlogPostDto>> GetBySlug(string slug)
    {
        // Cria a query
        var query = new GetBlogPostBySlugQuery(slug);
        
        // Envia para o MediatR processar
        // O handler já incrementa o ViewCount automaticamente
        var post = await _mediator.Send(query);
        
        // Se não encontrou, retorna 404
        if (post == null)
        {
            return NotFound(new { message = $"Post com slug '{slug}' não encontrado" });
        }
        
        // Retorna HTTP 200 OK com o post
        return Ok(post);
    }

    // ====================================
    // POST: api/blogposts
    // ====================================
    /// <summary>
    /// Cria um novo post
    /// </summary>
    /// <param name="createDto">Dados do post a ser criado</param>
    /// <returns>BlogPostDto do post criado</returns>
    /// <response code="201">Post criado com sucesso</response>
    /// <response code="400">Dados inválidos ou slug duplicado</response>
    [HttpPost]
    [ProducesResponseType(typeof(BlogPostDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<BlogPostDto>> Create([FromBody] CreateBlogPostDto createDto)
    {
        // Valida se o DTO veio preenchido
        if (createDto == null)
        {
            return BadRequest(new { message = "Dados do post são obrigatórios" });
        }

        try
        {
            // Cria o command
            var command = new CreateBlogPostCommand(createDto);
            
            // Envia para o MediatR processar
            // O Validator será executado automaticamente aqui
            var createdPost = await _mediator.Send(command);
            
            // Retorna HTTP 201 Created com o post criado
            // O header Location aponta para o endpoint admin (GetById é público e
            // filtrado por visibilidade, daria 404 para um post recém-criado como Draft)
            return CreatedAtAction(
                nameof(GetByIdAdmin),
                new { id = createdPost.Id },
                createdPost
            );
        }
        catch (InvalidOperationException ex)
        {
            // Slug duplicado
            return BadRequest(new { message = ex.Message });
        }
    }

    // ====================================
    // PUT: api/blogposts/{id}
    // ====================================
    /// <summary>
    /// Atualiza um post existente
    /// </summary>
    /// <param name="id">ID do post</param>
    /// <param name="updateDto">Dados atualizados</param>
    /// <returns>NoContent se sucesso</returns>
    /// <response code="204">Post atualizado com sucesso</response>
    /// <response code="400">Dados inválidos ou slug duplicado</response>
    /// <response code="404">Post não encontrado</response>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateBlogPostDto updateDto)
    {
        // Valida se o ID da URL bate com o ID do body
        if (id != updateDto.Id)
        {
            return BadRequest(new { message = "O ID da URL não corresponde ao ID do body" });
        }

        try
        {
            // Cria o command
            var command = new UpdateBlogPostCommand(updateDto);
            
            // Envia para o MediatR processar
            await _mediator.Send(command);
            
            // Retorna HTTP 204 No Content (sucesso, sem corpo)
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            // Slug duplicado
            return BadRequest(new { message = ex.Message });
        }
    }

    // ====================================
    // DELETE: api/blogposts/{id}
    // ====================================
    /// <summary>
    /// Deleta um post FISICAMENTE (hard delete)
    /// ATENÇÃO: Esta ação é irreversível!
    /// </summary>
    /// <param name="id">ID do post</param>
    /// <returns>NoContent se sucesso</returns>
    /// <response code="204">Post deletado com sucesso</response>
    /// <response code="404">Post não encontrado</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            // Cria o command
            var command = new DeleteBlogPostCommand(id);
            
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

    // ====================================
    // POST: api/blogposts/bulk-delete
    // ====================================
    /// <summary>
    /// Deleta vários posts FISICAMENTE (hard delete)
    /// ATENÇÃO: Esta ação é irreversível! Ids duplicados são ignorados e
    /// ids inexistentes não geram erro, apenas não contam
    /// </summary>
    /// <param name="dto">Lista de ids (1 a 100)</param>
    /// <returns>Quantidade de posts excluídos</returns>
    /// <response code="200">Retorna { "deletedCount": N }</response>
    /// <response code="400">Lista vazia, acima de 100 ids ou com Guid vazio</response>
    [HttpPost("bulk-delete")]
    [ProducesResponseType(typeof(BulkDeleteBlogPostsResultDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<BulkDeleteBlogPostsResultDto>> BulkDelete([FromBody] BulkDeleteBlogPostsDto dto)
    {
        if (dto == null)
        {
            return BadRequest(new { message = "Dados da exclusão são obrigatórios" });
        }

        // O Validator será executado automaticamente pelo ValidationBehavior
        var deleted = await _mediator.Send(new BulkDeleteBlogPostsCommand(dto));

        return Ok(new BulkDeleteBlogPostsResultDto { DeletedCount = deleted });
    }
}
