// ====================================
// Título: GetBlogPostByIdQuery.cs
// Descrição: Query para buscar um post específico por ID (CQRS - Read)
// Autor: Will
// Empresa: WpDev
// Data: 23/11/2024
// ====================================

using MediatR;
using Portfolio.Application.DTOs.BlogPosts;

namespace Portfolio.Application.Queries.BlogPosts.GetBlogPostById;

/// <summary>
/// Query para buscar um post específico por ID
/// Retorna BlogPostDto completo ou null se não encontrado
/// Usado no endpoint GET /api/blogposts/{id}
/// </summary>
public class GetBlogPostByIdQuery : IRequest<BlogPostDto?>
{
    // ====================================
    // PARÂMETRO DA QUERY
    // ====================================
    public Guid Id { get; set; }

    // ====================================
    // CONSTRUTOR
    // ====================================
    public GetBlogPostByIdQuery(Guid id)
    {
        Id = id;
    }
}