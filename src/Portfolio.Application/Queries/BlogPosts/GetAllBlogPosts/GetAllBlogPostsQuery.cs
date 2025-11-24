// ====================================
// Título: GetAllBlogPostsQuery.cs
// Descrição: Query para buscar todos os posts (CQRS - Read)
// Autor: Will
// Empresa: WpDev
// Data: 23/11/2024
// ====================================

using MediatR;
using Portfolio.Application.DTOs.BlogPosts;

namespace Portfolio.Application.Queries.BlogPosts.GetAllBlogPosts;

/// <summary>
/// Query para buscar TODOS os posts (publicados e rascunhos)
/// Retorna uma lista de BlogPostCardDto (versão simplificada para grid)
/// Usado no endpoint GET /api/blogposts
/// </summary>
public class GetAllBlogPostsQuery : IRequest<IEnumerable<BlogPostCardDto>>
{
    // Esta query não precisa de parâmetros
    // Ela simplesmente retorna TODOS os posts
}