// Título: GetLatestBlogPostsValidatorTests.cs
// Descrição: Testes unitários do validator da busca dos últimos posts públicos (Count de 1 a 12)

using FluentValidation.TestHelper;
using Portfolio.Application.Queries.BlogPosts.GetLatestBlogPosts;

namespace Portfolio.UnitTests.BlogPosts;

public class GetLatestBlogPostsQueryValidatorTests
{
    private readonly GetLatestBlogPostsQueryValidator _validator = new();

    [Theory]
    [InlineData(0)]
    [InlineData(13)]
    [InlineData(-1)]
    public void Validate_DeveFalhar_QuandoCountForaDoIntervalo(int count)
    {
        var resultado = _validator.TestValidate(new GetLatestBlogPostsQuery(count));

        resultado.ShouldHaveValidationErrorFor(x => x.Count)
            .WithErrorMessage("A quantidade de posts deve estar entre 1 e 12");
    }

    [Theory]
    [InlineData(1)]
    [InlineData(3)]
    [InlineData(12)]
    public void Validate_DevePassar_QuandoCountDentroDoIntervalo(int count)
    {
        var resultado = _validator.TestValidate(new GetLatestBlogPostsQuery(count));

        resultado.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_DevePassar_QuandoCountNaoInformado()
    {
        // O padrão da query é 3
        var resultado = _validator.TestValidate(new GetLatestBlogPostsQuery());

        resultado.ShouldNotHaveAnyValidationErrors();
    }
}
