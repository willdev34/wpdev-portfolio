// ====================================
// Título: GetBlogPostBySlugQuery.cs
// Descrição: Query para buscar um post por Slug/URL amigável (CQRS - Read)
// Autor: Will
// Empresa: WpDev
// Data: 23/11/2024
// ====================================

using MediatR;
using Portfolio.Application.DTOs.BlogPosts;

namespace Portfolio.Application.Queries.BlogPosts.GetBlogPostBySlug;

/// <summary>
/// Query para buscar um post específico por Slug (URL amigável)
/// Exemplo: /blog/como-aprender-dotnet ao invés de /blog/abc-123-guid
/// Retorna BlogPostDto completo ou null se não encontrado
/// Usado no endpoint GET /api/blogposts/slug/{slug}
/// </summary>
public class GetBlogPostBySlugQuery : IRequest<BlogPostDto?>
{
    // ====================================
    // PARÂMETRO DA QUERY
    // ====================================
    public string Slug { get; set; } = string.Empty;

    // ====================================
    // CONSTRUTOR
    // ====================================
    public GetBlogPostBySlugQuery(string slug)
    {
        Slug = slug;
    }
}