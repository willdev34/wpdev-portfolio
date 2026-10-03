// ====================================
// Título: BulkDeleteBlogPostsCommand.cs
// Descrição: Command para deletar vários posts de uma vez (hard delete - CQRS - Write)
// ====================================

using MediatR;
using Portfolio.Application.DTOs.BlogPosts;

namespace Portfolio.Application.Commands.BlogPosts.BulkDeleteBlogPosts;

/// <summary>
/// Command para deletar FISICAMENTE vários posts
/// Retorna a quantidade de posts realmente excluídos
/// Usado no endpoint POST /api/blogposts/bulk-delete
/// </summary>
public class BulkDeleteBlogPostsCommand : IRequest<int>
{
    public BulkDeleteBlogPostsDto Data { get; set; }

    public BulkDeleteBlogPostsCommand(BulkDeleteBlogPostsDto data)
    {
        Data = data;
    }
}
