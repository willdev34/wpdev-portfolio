// ====================================
// Título: UpdateBlogPostCommand.cs
// Descrição: Command para atualizar um post existente (CQRS - Write)
// Autor: Will
// Empresa: WpDev
// Data: 23/11/2024
// ====================================

using MediatR;
using Portfolio.Application.DTOs.BlogPosts;

namespace Portfolio.Application.Commands.BlogPosts.UpdateBlogPost;

/// <summary>
/// Command para atualizar um post existente
/// Recebe os dados do UpdateBlogPostDto
/// Retorna Unit (void) - não retorna nada em caso de sucesso
/// Usado no endpoint PUT /api/blogposts/{id}
/// </summary>
public class UpdateBlogPostCommand : IRequest<Unit>
{
    // ====================================
    // DADOS DO POST A SER ATUALIZADO
    // ====================================
    public UpdateBlogPostDto PostData { get; set; } = null!;

    // ====================================
    // CONSTRUTOR
    // ====================================
    public UpdateBlogPostCommand(UpdateBlogPostDto postData)
    {
        PostData = postData;
    }
}