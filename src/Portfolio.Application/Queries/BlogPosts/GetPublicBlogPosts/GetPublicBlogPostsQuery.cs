// ====================================
// Título: GetPublicBlogPostsQuery.cs
// Descrição: Query para buscar posts PÚBLICOS (Status == Published ou Scheduled vencido)
// ====================================

using MediatR;
using Portfolio.Application.DTOs.BlogPosts;

namespace Portfolio.Application.Queries.BlogPosts.GetPublicBlogPosts;

/// <summary>
/// Query para buscar posts PUBLICOS para exibicao publica
/// Filtra por Status == Published, ou Status == Scheduled com ScheduledAt ja vencido
/// Usado no site público (Blog.razor) para evitar vazamento de rascunhos e agendados futuros
/// </summary>
public record GetPublicBlogPostsQuery : IRequest<IEnumerable<BlogPostCardDto>>;
