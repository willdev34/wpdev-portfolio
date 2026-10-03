// ====================================
// Título: BulkDeleteBlogPostsCommandHandler.cs
// Descrição: Handler que processa BulkDeleteBlogPostsCommand (hard delete em lote)
// ====================================

using MediatR;
using Portfolio.Application.Interfaces;

namespace Portfolio.Application.Commands.BlogPosts.BulkDeleteBlogPosts;

/// <summary>
/// Handler responsável por processar o command BulkDeleteBlogPostsCommand
/// ATENÇÃO: Implementa HARD DELETE. Ids duplicados são ignorados e ids inexistentes
/// não geram erro, apenas não entram na contagem
/// </summary>
public class BulkDeleteBlogPostsCommandHandler : IRequestHandler<BulkDeleteBlogPostsCommand, int>
{
    private readonly IBlogPostRepository _repository;

    public BulkDeleteBlogPostsCommandHandler(IBlogPostRepository repository)
    {
        _repository = repository;
    }

    public async Task<int> Handle(
        BulkDeleteBlogPostsCommand request,
        CancellationToken cancellationToken)
    {
        var deleted = 0;

        foreach (var id in request.Data.Ids.Distinct())
        {
            if (!await _repository.ExistsAsync(id))
                continue;

            await _repository.DeleteAsync(id);
            deleted++;
        }

        // Um único SaveChanges para o lote inteiro (atômico)
        if (deleted > 0)
            await _repository.SaveChangesAsync();

        return deleted;
    }
}
