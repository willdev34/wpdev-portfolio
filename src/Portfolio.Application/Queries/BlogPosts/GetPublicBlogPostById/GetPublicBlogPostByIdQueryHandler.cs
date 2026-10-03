// ====================================
// Título: GetPublicBlogPostByIdQueryHandler.cs
// Descrição: Handler que processa GetPublicBlogPostByIdQuery, aplicando a regra pública
// ====================================

using AutoMapper;
using MediatR;
using Portfolio.Application.DTOs.BlogPosts;
using Portfolio.Application.Interfaces;

namespace Portfolio.Application.Queries.BlogPosts.GetPublicBlogPostById;

/// <summary>
/// Handler responsável por processar a query GetPublicBlogPostByIdQuery
/// 1. Busca o post no banco por Id
/// 2. Se nao existir ou nao for publico (Draft, ou Scheduled no futuro), retorna null
/// 3. Converte de Entity para DTO (via AutoMapper)
/// IMPORTANTE: usado apenas pelo endpoint publico. O admin usa GetBlogPostByIdQuery
/// (sem filtro) via GET /api/blogposts/admin/{id}
/// </summary>
public class GetPublicBlogPostByIdQueryHandler : IRequestHandler<GetPublicBlogPostByIdQuery, BlogPostDto?>
{
    private readonly IBlogPostRepository _repository;
    private readonly IMapper _mapper;

    public GetPublicBlogPostByIdQueryHandler(
        IBlogPostRepository repository,
        IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<BlogPostDto?> Handle(
        GetPublicBlogPostByIdQuery request,
        CancellationToken cancellationToken)
    {
        // 1. Busca o post no banco por Id
        var blogPost = await _repository.GetByIdAsync(request.Id);

        // 2. Se não encontrou OU não é público, retorna null (controller devolve 404)
        if (blogPost == null || !blogPost.IsPublic)
        {
            return null;
        }

        // 3. Converte de BlogPost (Entity) para BlogPostDto
        return _mapper.Map<BlogPostDto>(blogPost);
    }
}
