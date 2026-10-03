// ====================================
// Título: GetPublicBlogPostByIdQuery.cs
// Descrição: Query para buscar um post por Id, respeitando a regra de visibilidade pública
// ====================================

using MediatR;
using Portfolio.Application.DTOs.BlogPosts;

namespace Portfolio.Application.Queries.BlogPosts.GetPublicBlogPostById;

/// <summary>
/// Query para buscar um post por Id para exibicao PUBLICA
/// Retorna null se o post nao existir ou nao for publico (Draft, ou Scheduled no futuro)
/// Usado no endpoint publico e anonimo GET /api/blogposts/{id}
/// </summary>
public class GetPublicBlogPostByIdQuery : IRequest<BlogPostDto?>
{
    public Guid Id { get; set; }

    public GetPublicBlogPostByIdQuery(Guid id)
    {
        Id = id;
    }
}
