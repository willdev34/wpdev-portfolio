// ====================================
// Título: GetLatestBlogPostsQuery.cs
// Descrição: Query para buscar os últimos posts PÚBLICOS (prévia da Home)
// ====================================

using MediatR;
using Portfolio.Application.DTOs.BlogPosts;

namespace Portfolio.Application.Queries.BlogPosts.GetLatestBlogPosts;

/// <summary>
/// Query para buscar os N posts publicos mais recentes
/// Mesma regra publica: Status == Published, ou Scheduled com ScheduledAt ja vencido
/// Count deve estar entre 1 e 12 (padrao 3)
/// Usado no endpoint GET /api/blogposts/latest
/// </summary>
public record GetLatestBlogPostsQuery(int Count = 3) : IRequest<IEnumerable<BlogPostCardDto>>;
