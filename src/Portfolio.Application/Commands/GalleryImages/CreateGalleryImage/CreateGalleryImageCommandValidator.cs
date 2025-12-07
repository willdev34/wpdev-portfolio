// ====================================
// Título: CreateGalleryImageCommandValidator.cs
// Descrição: Validações para criação de imagem na galeria
// Autor: Will
// Empresa: WpDev
// Data: 06/12/2024
// ====================================

using FluentValidation;

namespace Portfolio.Application.Commands.GalleryImages.CreateGalleryImage;

/// <summary>
/// Validador para CreateGalleryImageCommand
/// Define todas as regras de validação para criação de imagens
/// Se alguma regra falhar, o Command não será processado
/// </summary>
public class CreateGalleryImageCommandValidator : AbstractValidator<CreateGalleryImageCommand>
{
    public CreateGalleryImageCommandValidator()
    {
        // VALIDAÇÃO DO TÍTULO
        RuleFor(x => x.ImageData.Title)
            .NotEmpty().WithMessage("O título da imagem é obrigatório")
            .MinimumLength(3).WithMessage("O título deve ter no mínimo 3 caracteres")
            .MaximumLength(200).WithMessage("O título deve ter no máximo 200 caracteres");

        // VALIDAÇÃO DO ALT TEXT (acessibilidade)
        RuleFor(x => x.ImageData.AltText)
            .NotEmpty().WithMessage("O texto alternativo (Alt Text) é obrigatório para acessibilidade")
            .MinimumLength(5).WithMessage("O Alt Text deve ter no mínimo 5 caracteres")
            .MaximumLength(200).WithMessage("O Alt Text deve ter no máximo 200 caracteres");

        // VALIDAÇÃO DA DESCRIÇÃO (opcional)
        RuleFor(x => x.ImageData.Description)
            .MaximumLength(500).WithMessage("A descrição deve ter no máximo 500 caracteres")
            .When(x => !string.IsNullOrEmpty(x.ImageData.Description));

        // VALIDAÇÃO DA URL DA IMAGEM
        RuleFor(x => x.ImageData.ImageUrl)
            .NotEmpty().WithMessage("A URL da imagem é obrigatória")
            .MaximumLength(500).WithMessage("A URL da imagem deve ter no máximo 500 caracteres")
            .Must(BeAValidUrl).WithMessage("A URL da imagem deve ser válida");

        // VALIDAÇÃO DA URL DO THUMBNAIL (opcional)
        RuleFor(x => x.ImageData.ThumbnailUrl)
            .MaximumLength(500).WithMessage("A URL do thumbnail deve ter no máximo 500 caracteres")
            .Must(BeAValidUrl).WithMessage("A URL do thumbnail deve ser válida")
            .When(x => !string.IsNullOrEmpty(x.ImageData.ThumbnailUrl));

        // VALIDAÇÃO DAS TAGS
        RuleFor(x => x.ImageData.Tags)
            .Must(x => x.Count <= 10).WithMessage("Informe no máximo 10 tags")
            .When(x => x.ImageData.Tags != null && x.ImageData.Tags.Any());

        RuleForEach(x => x.ImageData.Tags)
            .MaximumLength(50).WithMessage("Cada tag deve ter no máximo 50 caracteres")
            .When(x => x.ImageData.Tags != null && x.ImageData.Tags.Any());

        // VALIDAÇÃO DAS DIMENSÕES
        RuleFor(x => x.ImageData.Width)
            .GreaterThan(0).WithMessage("A largura deve ser maior que 0")
            .LessThanOrEqualTo(10000).WithMessage("A largura deve ser no máximo 10000 pixels");

        RuleFor(x => x.ImageData.Height)
            .GreaterThan(0).WithMessage("A altura deve ser maior que 0")
            .LessThanOrEqualTo(10000).WithMessage("A altura deve ser no máximo 10000 pixels");

        // VALIDAÇÃO DO TAMANHO DO ARQUIVO
        RuleFor(x => x.ImageData.FileSizeBytes)
            .GreaterThan(0).WithMessage("O tamanho do arquivo deve ser maior que 0")
            .LessThanOrEqualTo(10485760).WithMessage("O tamanho do arquivo deve ser no máximo 10MB (10485760 bytes)");

        // VALIDAÇÃO DA ORDEM
        RuleFor(x => x.ImageData.Order)
            .GreaterThanOrEqualTo(0).WithMessage("A ordem deve ser maior ou igual a 0");
    }

    private bool BeAValidUrl(string? url)
    {
        if (string.IsNullOrEmpty(url))
            return true;

        return Uri.TryCreate(url, UriKind.Absolute, out var uriResult)
               && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
    }
}