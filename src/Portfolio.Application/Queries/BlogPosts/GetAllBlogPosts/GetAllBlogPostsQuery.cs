// ====================================
// Título: GetAllBlogPostsQuery.cs
// Descrição: Query para buscar todos os posts (CQRS - Read)
// ====================================

using MediatR;
using Portfolio.Application.DTOs.BlogPosts;

namespace Portfolio.Application.Queries.BlogPosts.GetAllBlogPosts;

/// <summary>
/// Query para buscar TODOS os posts (publicados e rascunhos)
/// Retorna uma lista de BlogPostAdminCardDto (versão simplificada para grid, com Status e ScheduledAt)
/// Usado no endpoint GET /api/blogposts (ADMIN)
/// </summary>
public class GetAllBlogPostsQuery : IRequest<IEnumerable<BlogPostAdminCardDto>>
{
    // Esta query não precisa de parâmetros
    // Ela simplesmente retorna TODOS os posts
}