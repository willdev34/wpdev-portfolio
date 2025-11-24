// ====================================
// Título: DeleteBlogPostCommand.cs
// Descrição: Command para deletar um post (hard delete - CQRS - Write)
// Autor: Will
// Empresa: WpDev
// Data: 23/11/2024
// ====================================

using MediatR;

namespace Portfolio.Application.Commands.BlogPosts.DeleteBlogPost;

/// <summary>
/// Command para deletar um post FISICAMENTE do banco
/// ATENÇÃO: BlogPost usa HARD DELETE (ao contrário de Project que usa soft delete)
/// Recebe apenas o ID do post a ser deletado
/// Retorna Unit (void)
/// Usado no endpoint DELETE /api/blogposts/{id}
/// </summary>
public class DeleteBlogPostCommand : IRequest<Unit>
{
    // ====================================
    // ID DO POST A SER DELETADO
    // ====================================
    public Guid Id { get; set; }

    // ====================================
    // CONSTRUTOR
    // ====================================
    public DeleteBlogPostCommand(Guid id)
    {
        Id = id;
    }
}