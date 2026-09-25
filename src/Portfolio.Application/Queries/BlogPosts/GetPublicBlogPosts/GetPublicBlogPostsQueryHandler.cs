// ====================================
// Título: GetPublicBlogPostsQueryHandler.cs
// Descrição: Handler que retorna apenas posts PUBLICADOS
// ====================================

using AutoMapper;
using MediatR;
using Portfolio.Application.DTOs.BlogPosts;
using Portfolio.Application.Interfaces;

namespace Portfolio.Application.Queries.BlogPosts.GetPublicBlogPosts;

/// <summary>
/// Handler responsável por processar a query GetPublicBlogPostsQuery
/// 1. Busca apenas posts PUBLICADOS no banco (IsPublished = true)
/// 2. Converte de Entity para DTO (via AutoMapper)
/// 3. Retorna a lista de BlogPostCardDto
/// IMPORTANTE: Este handler é usado no site PÚBLICO e NÃO retorna rascunhos
/// </summary>
public class GetPublicBlogPostsQueryHandler : IRequestHandler<GetPublicBlogPostsQuery, IEnumerable<BlogPostCardDto>>
{
    private readonly IBlogPostRepository _repository;
    private readonly IMapper _mapper;

    // ====================================
    // CONSTRUTOR - Injeção de Dependência
    // ====================================
    public GetPublicBlogPostsQueryHandler(
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
    public async Task<IEnumerable<BlogPostCardDto>> Handle(
        GetPublicBlogPostsQuery request,
        CancellationToken cancellationToken)
    {
        // 1. Busca APENAS posts publicados do banco (IsPublished = true)
        // Usa o método GetPublishedAsync() que já existe no Repository
        var blogPosts = await _repository.GetPublishedAsync();

        // 2. Converte de BlogPost (Entity) para BlogPostCardDto
        var blogPostCards = _mapper.Map<IEnumerable<BlogPostCardDto>>(blogPosts);

        // 3. Retorna a lista de DTOs
        return blogPostCards;
    }
}
