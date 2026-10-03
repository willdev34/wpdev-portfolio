// ====================================
// Título: GetLatestBlogPostsQueryValidator.cs
// Descrição: Validações para a busca dos últimos posts públicos
// ====================================

using FluentValidation;

namespace Portfolio.Application.Queries.BlogPosts.GetLatestBlogPosts;

/// <summary>
/// Validador para GetLatestBlogPostsQuery: Count entre 1 e 12
/// </summary>
public class GetLatestBlogPostsQueryValidator : AbstractValidator<GetLatestBlogPostsQuery>
{
    public const int MaxCount = 12;

    public GetLatestBlogPostsQueryValidator()
    {
        RuleFor(x => x.Count)
            .InclusiveBetween(1, MaxCount)
            .WithMessage($"A quantidade de posts deve estar entre 1 e {MaxCount}");
    }
}
