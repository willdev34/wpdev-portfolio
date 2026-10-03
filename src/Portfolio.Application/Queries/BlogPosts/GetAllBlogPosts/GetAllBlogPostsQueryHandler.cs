// ====================================
// Título: GetAllBlogPostsQueryHandler.cs
// Descrição: Handler que processa GetAllBlogPostsQuery e retorna os posts
// ====================================

using AutoMapper;
using MediatR;
using Portfolio.Application.DTOs.BlogPosts;
using Portfolio.Application.Interfaces;

namespace Portfolio.Application.Queries.BlogPosts.GetAllBlogPosts;

/// <summary>
/// Handler responsável por processar a query GetAllBlogPostsQuery
/// 1. Busca os posts no banco (via Repository)
/// 2. Converte de Entity para DTO (via AutoMapper)
/// 3. Retorna a lista de BlogPostAdminCardDto (inclui Status e ScheduledAt, só para o admin)
/// </summary>
public class GetAllBlogPostsQueryHandler : IRequestHandler<GetAllBlogPostsQuery, IEnumerable<BlogPostAdminCardDto>>
{
    private readonly IBlogPostRepository _repository;
    private readonly IMapper _mapper;

    // ====================================
    // CONSTRUTOR - Injeção de Dependência
    // ====================================
    public GetAllBlogPostsQueryHandler(
        IBlogPostRepository repository,
        IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    // ====================================
    // HANDLE - Processa a Query
    // ====================================
    /// <summary>
    /// Método principal que executa a lógica da query
    /// </summary>
    public async Task<IEnumerable<BlogPostAdminCardDto>> Handle(
        GetAllBlogPostsQuery request,
        CancellationToken cancellationToken)
    {
        // 1. Busca TODOS os posts do banco
        var blogPosts = await _repository.GetAllAsync();

        // 2. Converte de BlogPost (Entity) para BlogPostAdminCardDto
        var blogPostCards = _mapper.Map<IEnumerable<BlogPostAdminCardDto>>(blogPosts);

        // 3. Retorna a lista de DTOs
        return blogPostCards;
    }
}