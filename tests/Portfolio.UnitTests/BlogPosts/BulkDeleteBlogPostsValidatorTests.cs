// Título: BulkDeleteBlogPostsValidatorTests.cs
// Descrição: Testes unitários do validator da exclusão em massa de posts

using FluentValidation.TestHelper;
using Portfolio.Application.Commands.BlogPosts.BulkDeleteBlogPosts;
using Portfolio.Application.DTOs.BlogPosts;

namespace Portfolio.UnitTests.BlogPosts;

public class BulkDeleteBlogPostsCommandValidatorTests
{
    private readonly BulkDeleteBlogPostsCommandValidator _validator = new();

    private static BulkDeleteBlogPostsCommand BuildCommand(IEnumerable<Guid> ids) =>
        new(new BulkDeleteBlogPostsDto { Ids = ids.ToList() });

    [Fact]
    public void Validate_DeveFalhar_QuandoListaVazia()
    {
        var resultado = _validator.TestValidate(BuildCommand(Array.Empty<Guid>()));

        resultado.ShouldHaveValidationErrorFor(x => x.Data.Ids)
            .WithErrorMessage("Informe ao menos um post para excluir");
    }

    [Fact]
    public void Validate_DeveFalhar_QuandoMaisDeCemIds()
    {
        var ids = Enumerable.Range(0, 101).Select(_ => Guid.NewGuid());

        var resultado = _validator.TestValidate(BuildCommand(ids));

        resultado.ShouldHaveValidationErrorFor(x => x.Data.Ids)
            .WithErrorMessage("Informe no máximo 100 posts por exclusão");
    }

    [Fact]
    public void Validate_DeveFalhar_QuandoListaContemGuidVazio()
    {
        var resultado = _validator.TestValidate(BuildCommand(new[] { Guid.NewGuid(), Guid.Empty }));

        resultado.ShouldHaveValidationErrorFor("Data.Ids[1]")
            .WithErrorMessage("Os ids informados não podem ser vazios");
    }

    [Fact]
    public void Validate_DevePassar_QuandoListaValida()
    {
        var resultado = _validator.TestValidate(BuildCommand(new[] { Guid.NewGuid(), Guid.NewGuid() }));

        resultado.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_DevePassar_QuandoExatamenteCemIds()
    {
        var ids = Enumerable.Range(0, 100).Select(_ => Guid.NewGuid());

        var resultado = _validator.TestValidate(BuildCommand(ids));

        resultado.ShouldNotHaveAnyValidationErrors();
    }
}
