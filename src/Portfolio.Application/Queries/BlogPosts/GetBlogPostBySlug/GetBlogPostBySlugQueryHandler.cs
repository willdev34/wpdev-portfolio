// ====================================
// Título: GetBlogPostBySlugQueryHandler.cs
// Descrição: Handler que processa GetBlogPostBySlugQuery e retorna o post
// Autor: Will
// Empresa: WpDev
// Data: 23/11/2024
// ====================================

using AutoMapper;
using MediatR;
using Portfolio.Application.DTOs.BlogPosts;
using Portfolio.Application.Interfaces;

namespace Portfolio.Application.Queries.BlogPosts.GetBlogPostBySlug;

/// <summary>
/// Handler responsável por processar a query GetBlogPostBySlugQuery
/// 1. Busca o post no banco por Slug (via Repository)
/// 2. Incrementa o contador de visualizações (ViewCount)
/// 3. Converte de Entity para DTO (via AutoMapper)
/// 4. Retorna o BlogPostDto ou null se não encontrado
/// </summary>
public class GetBlogPostBySlugQueryHandler : IRequestHandler<GetBlogPostBySlugQuery, BlogPostDto?>
{
    private readonly IBlogPostRepository _repository;
    private readonly IMapper _mapper;

    // ====================================
    // CONSTRUTOR - Injeção de Dependência
    // ====================================
    public GetBlogPostBySlugQueryHandler(
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
    public async Task<BlogPostDto?> Handle(
        GetBlogPostBySlugQuery request, 
        CancellationToken cancellationToken)
    {
        // 1. Busca o post no banco por Slug
        var blogPost = await _repository.GetBySlugAsync(request.Slug);

        // 2. Se não encontrou, retorna null
        if (blogPost == null)
        {
            return null;
        }

        // 3. Incrementa o contador de visualizações
        // Isso registra que alguém acessou o post
        await _repository.IncrementViewCountAsync(blogPost.Id);
        await _repository.SaveChangesAsync();

        // 4. Converte de BlogPost (Entity) para BlogPostDto
        var blogPostDto = _mapper.Map<BlogPostDto>(blogPost);

        // 5. Retorna o DTO
        return blogPostDto;
    }
}