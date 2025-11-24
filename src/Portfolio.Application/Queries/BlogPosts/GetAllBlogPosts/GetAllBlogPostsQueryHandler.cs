// ====================================
// Título: GetAllBlogPostsQueryHandler.cs
// Descrição: Handler que processa GetAllBlogPostsQuery e retorna os posts
// Autor: Will
// Empresa: WpDev
// Data: 23/11/2024
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
/// 3. Retorna a lista de BlogPostCardDto
/// </summary>
public class GetAllBlogPostsQueryHandler : IRequestHandler<GetAllBlogPostsQuery, IEnumerable<BlogPostCardDto>>
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
    public async Task<IEnumerable<BlogPostCardDto>> Handle(
        GetAllBlogPostsQuery request, 
        CancellationToken cancellationToken)
    {
        // 1. Busca TODOS os posts do banco
        var blogPosts = await _repository.GetAllAsync();

        // 2. Converte de BlogPost (Entity) para BlogPostCardDto
        var blogPostCards = _mapper.Map<IEnumerable<BlogPostCardDto>>(blogPosts);

        // 3. Retorna a lista de DTOs
        return blogPostCards;
    }
}