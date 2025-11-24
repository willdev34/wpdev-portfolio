// ====================================
// Título: CreateBlogPostCommand.cs
// Descrição: Command para criar um novo post do blog (CQRS - Write)
// Autor: Will
// Empresa: WpDev
// Data: 23/11/2024
// ====================================

using MediatR;
using Portfolio.Application.DTOs.BlogPosts;

namespace Portfolio.Application.Commands.BlogPosts.CreateBlogPost;

/// <summary>
/// Command para criar um novo post do blog
/// Recebe os dados do CreateBlogPostDto e retorna o BlogPostDto completo do post criado
/// Usado no endpoint POST /api/blogposts
/// </summary>
public class CreateBlogPostCommand : IRequest<BlogPostDto>
{
    // ====================================
    // DADOS DO POST A SER CRIADO
    // ====================================
    public CreateBlogPostDto PostData { get; set; } = null!;

    // ====================================
    // CONSTRUTOR
    // ====================================
    public CreateBlogPostCommand(CreateBlogPostDto postData)
    {
        PostData = postData;
    }
}