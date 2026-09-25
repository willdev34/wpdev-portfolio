// ====================================
// Título: GetPublicBlogPostsQuery.cs
// Descrição: Query para buscar posts PÚBLICOS (IsPublished = true)
// ====================================

using MediatR;
using Portfolio.Application.DTOs.BlogPosts;

namespace Portfolio.Application.Queries.BlogPosts.GetPublicBlogPosts;

/// <summary>
/// Query para buscar posts PUBLICADOS para exibição pública
/// Filtra apenas posts com IsPublished = true e PublishedAt != null
/// Usado no site público (Blog.razor) para evitar vazamento de rascunhos
/// </summary>
public record GetPublicBlogPostsQuery : IRequest<IEnumerable<BlogPostCardDto>>;
