// Título: BlogPostValidatorTests.cs
// Descrição: Testes unitários dos validators de BlogPosts (Create e Update), foco em Status e ScheduledAt

using FluentAssertions;
using FluentValidation.TestHelper;
using Portfolio.Application.Commands.BlogPosts.CreateBlogPost;
using Portfolio.Application.Commands.BlogPosts.UpdateBlogPost;
using Portfolio.Application.DTOs.BlogPosts;
using Portfolio.Domain.Enums;

namespace Portfolio.UnitTests.BlogPosts;

public class CreateBlogPostCommandValidatorTests
{
    private readonly CreateBlogPostCommandValidator _validator = new();

    [Fact]
    public void Validate_DeveFalhar_QuandoScheduledSemScheduledAt()
    {
        // Arrange
        var dto = BuildValidDto();
        dto.Status = BlogPostStatus.Scheduled;
        dto.ScheduledAt = null;

        // Act
        var resultado = _validator.TestValidate(new CreateBlogPostCommand(dto));

        // Assert
        resultado.ShouldHaveValidationErrorFor(x => x.PostData.ScheduledAt)
            .WithErrorMessage("A data de agendamento é obrigatória quando o status é Scheduled");
    }

    [Fact]
    public void Validate_DeveFalhar_QuandoScheduledComScheduledAtNoPassado()
    {
        // Arrange
        var dto = BuildValidDto();
        dto.Status = BlogPostStatus.Scheduled;
        dto.ScheduledAt = DateTime.UtcNow.AddDays(-1);

        // Act
        var resultado = _validator.TestValidate(new CreateBlogPostCommand(dto));

        // Assert
        resultado.ShouldHaveValidationErrorFor(x => x.PostData.ScheduledAt)
            .WithErrorMessage("A data de agendamento deve ser no futuro");
    }

    [Fact]
    public void Validate_DevePassar_QuandoScheduledComScheduledAtNoFuturo()
    {
        // Arrange
        var dto = BuildValidDto();
        dto.Status = BlogPostStatus.Scheduled;
        dto.ScheduledAt = DateTime.UtcNow.AddDays(1);

        // Act
        var resultado = _validator.TestValidate(new CreateBlogPostCommand(dto));

        // Assert
        resultado.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_DevePassar_QuandoPublishedComScheduledAtPreenchido()
    {
        // Arrange
        // ScheduledAt só é validado quando Status = Scheduled; o handler descarta o valor em Published
        var dto = BuildValidDto();
        dto.Status = BlogPostStatus.Published;
        dto.ScheduledAt = DateTime.UtcNow.AddDays(-1);

        // Act
        var resultado = _validator.TestValidate(new CreateBlogPostCommand(dto));

        // Assert
        resultado.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_DeveFalhar_QuandoStatusForaDoEnum()
    {
        // Arrange
        var dto = BuildValidDto();
        dto.Status = (BlogPostStatus)99;

        // Act
        var resultado = _validator.TestValidate(new CreateBlogPostCommand(dto));

        // Assert
        resultado.ShouldHaveValidationErrorFor(x => x.PostData.Status)
            .WithErrorMessage("O status informado é inválido");
    }

    [Fact]
    public void Validate_DevePassar_QuandoStatusNulo()
    {
        // Arrange
        // Status nulo: fallback por IsPublished no handler
        var dto = BuildValidDto();
        dto.Status = null;
        dto.ScheduledAt = null;

        // Act
        var resultado = _validator.TestValidate(new CreateBlogPostCommand(dto));

        // Assert
        resultado.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_DeveFalhar_QuandoFeaturedImageUrlInvalida()
    {
        // Arrange
        var dto = BuildValidDto();
        dto.FeaturedImageUrl = "ftp://imagem.png";

        // Act
        var resultado = _validator.TestValidate(new CreateBlogPostCommand(dto));

        // Assert
        resultado.ShouldHaveValidationErrorFor(x => x.PostData.FeaturedImageUrl)
            .WithErrorMessage("A URL da imagem deve ser válida");
    }

    [Fact]
    public void Validate_DevePassar_QuandoFeaturedImageUrlHttps()
    {
        // Arrange
        var dto = BuildValidDto();
        dto.FeaturedImageUrl = "https://res.cloudinary.com/demo/image/upload/capa.png";

        // Act
        var resultado = _validator.TestValidate(new CreateBlogPostCommand(dto));

        // Assert
        resultado.IsValid.Should().BeTrue();
    }

    private static CreateBlogPostDto BuildValidDto() => new()
    {
        Title = "Clean Architecture em .NET",
        Slug = "clean-architecture-em-dotnet",
        Excerpt = "Resumo do post com mais de dez caracteres.",
        Content = new string('x', 60),
        Tags = new List<string> { "dotnet", "arquitetura" },
        IsPublished = true,
        ReadTimeMinutes = 5
    };
}

public class UpdateBlogPostCommandValidatorTests
{
    private readonly UpdateBlogPostCommandValidator _validator = new();

    [Fact]
    public void Validate_DeveFalhar_QuandoScheduledSemScheduledAt()
    {
        // Arrange
        var dto = BuildValidDto();
        dto.Status = BlogPostStatus.Scheduled;
        dto.ScheduledAt = null;

        // Act
        var resultado = _validator.TestValidate(new UpdateBlogPostCommand(dto));

        // Assert
        resultado.ShouldHaveValidationErrorFor(x => x.PostData.ScheduledAt)
            .WithErrorMessage("A data de agendamento é obrigatória quando o status é Scheduled");
    }

    [Fact]
    public void Validate_DeveFalhar_QuandoScheduledComScheduledAtNoPassado()
    {
        // Arrange
        var dto = BuildValidDto();
        dto.Status = BlogPostStatus.Scheduled;
        dto.ScheduledAt = DateTime.UtcNow.AddDays(-1);

        // Act
        var resultado = _validator.TestValidate(new UpdateBlogPostCommand(dto));

        // Assert
        resultado.ShouldHaveValidationErrorFor(x => x.PostData.ScheduledAt)
            .WithErrorMessage("A data de agendamento deve ser no futuro");
    }

    [Fact]
    public void Validate_DevePassar_QuandoScheduledComScheduledAtNoFuturo()
    {
        // Arrange
        var dto = BuildValidDto();
        dto.Status = BlogPostStatus.Scheduled;
        dto.ScheduledAt = DateTime.UtcNow.AddDays(1);

        // Act
        var resultado = _validator.TestValidate(new UpdateBlogPostCommand(dto));

        // Assert
        resultado.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_DevePassar_QuandoPublishedComScheduledAtPreenchido()
    {
        // Arrange
        var dto = BuildValidDto();
        dto.Status = BlogPostStatus.Published;
        dto.ScheduledAt = DateTime.UtcNow.AddDays(-1);

        // Act
        var resultado = _validator.TestValidate(new UpdateBlogPostCommand(dto));

        // Assert
        resultado.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_DeveFalhar_QuandoStatusForaDoEnum()
    {
        // Arrange
        var dto = BuildValidDto();
        dto.Status = (BlogPostStatus)99;

        // Act
        var resultado = _validator.TestValidate(new UpdateBlogPostCommand(dto));

        // Assert
        resultado.ShouldHaveValidationErrorFor(x => x.PostData.Status)
            .WithErrorMessage("O status informado é inválido");
    }

    [Fact]
    public void Validate_DevePassar_QuandoStatusNulo()
    {
        // Arrange
        var dto = BuildValidDto();
        dto.Status = null;
        dto.ScheduledAt = null;

        // Act
        var resultado = _validator.TestValidate(new UpdateBlogPostCommand(dto));

        // Assert
        resultado.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_DeveFalhar_QuandoFeaturedImageUrlInvalida()
    {
        // Arrange
        var dto = BuildValidDto();
        dto.FeaturedImageUrl = "ftp://imagem.png";

        // Act
        var resultado = _validator.TestValidate(new UpdateBlogPostCommand(dto));

        // Assert
        resultado.ShouldHaveValidationErrorFor(x => x.PostData.FeaturedImageUrl)
            .WithErrorMessage("A URL da imagem deve ser válida");
    }

    [Fact]
    public void Validate_DevePassar_QuandoFeaturedImageUrlHttps()
    {
        // Arrange
        var dto = BuildValidDto();
        dto.FeaturedImageUrl = "https://res.cloudinary.com/demo/image/upload/capa.png";

        // Act
        var resultado = _validator.TestValidate(new UpdateBlogPostCommand(dto));

        // Assert
        resultado.IsValid.Should().BeTrue();
    }

    private static UpdateBlogPostDto BuildValidDto() => new()
    {
        Id = Guid.NewGuid(),
        Title = "Clean Architecture em .NET",
        Slug = "clean-architecture-em-dotnet",
        Excerpt = "Resumo do post com mais de dez caracteres.",
        Content = new string('x', 60),
        Tags = new List<string> { "dotnet", "arquitetura" },
        IsPublished = true,
        ReadTimeMinutes = 5
    };
}
