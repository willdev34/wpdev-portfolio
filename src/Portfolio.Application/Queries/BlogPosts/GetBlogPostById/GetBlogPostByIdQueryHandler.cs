// ====================================
// Título: GetBlogPostByIdQueryHandler.cs
// Descrição: Handler que processa GetBlogPostByIdQuery e retorna o post
// Autor: Will
// Empresa: WpDev
// Data: 23/11/2024
// ====================================

using AutoMapper;
using MediatR;
using Portfolio.Application.DTOs.BlogPosts;
using Portfolio.Application.Interfaces;

namespace Portfolio.Application.Queries.BlogPosts.GetBlogPostById;

/// <summary>
/// Handler responsável por processar a query GetBlogPostByIdQuery
/// 1. Busca o post no banco por ID (via Repository)
/// 2. Converte de Entity para DTO (via AutoMapper)
/// 3. Retorna o BlogPostDto ou null se não encontrado
/// </summary>
public class GetBlogPostByIdQueryHandler : IRequestHandler<GetBlogPostByIdQuery, BlogPostDto?>
{
    private readonly IBlogPostRepository _repository;
    private readonly IMapper _mapper;

    // ====================================
    // CONSTRUTOR - Injeção de Dependência
    // ====================================
    public GetBlogPostByIdQueryHandler(
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
        GetBlogPostByIdQuery request, 
        CancellationToken cancellationToken)
    {
        // 1. Busca o post no banco por ID
        var blogPost = await _repository.GetByIdAsync(request.Id);

        // 2. Se não encontrou, retorna null
        if (blogPost == null)
        {
            return null;
        }

        // 3. Converte de BlogPost (Entity) para BlogPostDto
        var blogPostDto = _mapper.Map<BlogPostDto>(blogPost);

        // 4. Retorna o DTO
        return blogPostDto;
    }
}