// ====================================
// Título: CreateBlogPostCommandValidator.cs
// Descrição: Validações para criação de post do blog
// Autor: Will
// Empresa: WpDev
// Data: 23/11/2024
// ====================================

using FluentValidation;

namespace Portfolio.Application.Commands.BlogPosts.CreateBlogPost;

/// <summary>
/// Validador para CreateBlogPostCommand
/// Define todas as regras de validação para criação de posts
/// Se alguma regra falhar, o Command não será processado
/// </summary>
public class CreateBlogPostCommandValidator : AbstractValidator<CreateBlogPostCommand>
{
    public CreateBlogPostCommandValidator()
    {
        // ====================================
        // VALIDAÇÃO DO TÍTULO
        // ====================================
        RuleFor(x => x.PostData.Title)
            .NotEmpty().WithMessage("O título do post é obrigatório")
            .MinimumLength(3).WithMessage("O título deve ter no mínimo 3 caracteres")
            .MaximumLength(200).WithMessage("O título deve ter no máximo 200 caracteres");

        // ====================================
        // VALIDAÇÃO DO SLUG
        // ====================================
        RuleFor(x => x.PostData.Slug)
            .NotEmpty().WithMessage("O slug é obrigatório")
            .MinimumLength(3).WithMessage("O slug deve ter no mínimo 3 caracteres")
            .MaximumLength(200).WithMessage("O slug deve ter no máximo 200 caracteres")
            .Matches(@"^[a-z0-9]+(?:-[a-z0-9]+)*$")
                .WithMessage("O slug deve conter apenas letras minúsculas, números e hífens (ex: meu-post-123)");

        // ====================================
        // VALIDAÇÃO DO EXCERPT (Resumo)
        // ====================================
        RuleFor(x => x.PostData.Excerpt)
            .NotEmpty().WithMessage("O resumo do post é obrigatório")
            .MinimumLength(10).WithMessage("O resumo deve ter no mínimo 10 caracteres")
            .MaximumLength(500).WithMessage("O resumo deve ter no máximo 500 caracteres");

        // ====================================
        // VALIDAÇÃO DO CONTEÚDO
        // ====================================
        RuleFor(x => x.PostData.Content)
            .NotEmpty().WithMessage("O conteúdo do post é obrigatório")
            .MinimumLength(50).WithMessage("O conteúdo deve ter no mínimo 50 caracteres");

        // ====================================
        // VALIDAÇÃO DA IMAGEM DESTAQUE (OPCIONAL)
        // ====================================
        RuleFor(x => x.PostData.FeaturedImageUrl)
            .MaximumLength(500).WithMessage("A URL da imagem deve ter no máximo 500 caracteres")
            .Must(BeAValidUrl).WithMessage("A URL da imagem deve ser válida")
            .When(x => !string.IsNullOrEmpty(x.PostData.FeaturedImageUrl));

        // ====================================
        // VALIDAÇÃO DAS TAGS
        // ====================================
        RuleFor(x => x.PostData.Tags)
            .Must(x => x.Count <= 10).WithMessage("Informe no máximo 10 tags")
            .When(x => x.PostData.Tags != null && x.PostData.Tags.Any());

        RuleForEach(x => x.PostData.Tags)
            .MaximumLength(50).WithMessage("Cada tag deve ter no máximo 50 caracteres")
            .When(x => x.PostData.Tags != null && x.PostData.Tags.Any());

        // ====================================
        // VALIDAÇÃO DO TEMPO DE LEITURA
        // ====================================
        RuleFor(x => x.PostData.ReadTimeMinutes)
            .GreaterThan(0).WithMessage("O tempo de leitura deve ser maior que 0")
            .LessThanOrEqualTo(300).WithMessage("O tempo de leitura deve ser no máximo 300 minutos (5 horas)");
    }

    // ====================================
    // MÉTODO AUXILIAR - Validar URL
    // ====================================
    /// <summary>
    /// Valida se a string é uma URL válida
    /// </summary>
    private bool BeAValidUrl(string? url)
    {
        if (string.IsNullOrEmpty(url))
            return true; // URLs opcionais podem ser vazias

        return Uri.TryCreate(url, UriKind.Absolute, out var uriResult)
               && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
    }
}