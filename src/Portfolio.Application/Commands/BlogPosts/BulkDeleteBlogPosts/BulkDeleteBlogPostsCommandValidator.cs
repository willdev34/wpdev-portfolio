// ====================================
// Título: BulkDeleteBlogPostsCommandValidator.cs
// Descrição: Validações para exclusão em massa de posts do blog
// ====================================

using FluentValidation;

namespace Portfolio.Application.Commands.BlogPosts.BulkDeleteBlogPosts;

/// <summary>
/// Validador para BulkDeleteBlogPostsCommand
/// </summary>
public class BulkDeleteBlogPostsCommandValidator : AbstractValidator<BulkDeleteBlogPostsCommand>
{
    public const int MaxIds = 100;

    public BulkDeleteBlogPostsCommandValidator()
    {
        RuleFor(x => x.Data.Ids)
            .NotEmpty().WithMessage("Informe ao menos um post para excluir")
            .Must(ids => ids == null || ids.Count <= MaxIds)
                .WithMessage($"Informe no máximo {MaxIds} posts por exclusão");

        RuleForEach(x => x.Data.Ids)
            .NotEqual(Guid.Empty).WithMessage("Os ids informados não podem ser vazios");
    }
}
