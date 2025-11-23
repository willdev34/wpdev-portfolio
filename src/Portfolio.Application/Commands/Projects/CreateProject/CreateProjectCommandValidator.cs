// ====================================
// Título: CreateProjectCommandValidator.cs (REFATORADO)
// Descrição: Validações para criação de projeto - Design Editorial
// Autor: Will
// Empresa: WpDev
// Data: 23/11/2024
// ====================================

using FluentValidation;

namespace Portfolio.Application.Commands.Projects.CreateProject;

/// <summary>
/// Validador para CreateProjectCommand
/// Define todas as regras de validação para o design editorial minimalista
/// Se alguma regra falhar, o Command não será processado
/// </summary>
public class CreateProjectCommandValidator : AbstractValidator<CreateProjectCommand>
{
    public CreateProjectCommandValidator()
    {
        // ====================================
        // VALIDAÇÃO DO TÍTULO
        // ====================================
        RuleFor(x => x.ProjectData.Title)
            .NotEmpty().WithMessage("O título do projeto é obrigatório")
            .MinimumLength(3).WithMessage("O título deve ter no mínimo 3 caracteres")
            .MaximumLength(200).WithMessage("O título deve ter no máximo 200 caracteres");

        // ====================================
        // VALIDAÇÃO DA DESCRIÇÃO
        // ====================================
        RuleFor(x => x.ProjectData.Description)
            .NotEmpty().WithMessage("A descrição do projeto é obrigatória")
            .MinimumLength(10).WithMessage("A descrição deve ter no mínimo 10 caracteres")
            .MaximumLength(2000).WithMessage("A descrição deve ter no máximo 2000 caracteres");

        // ====================================
        // VALIDAÇÃO DA DESCRIÇÃO CURTA (OPCIONAL)
        // ====================================
        RuleFor(x => x.ProjectData.ShortDescription)
            .MaximumLength(500).WithMessage("A descrição curta deve ter no máximo 500 caracteres")
            .When(x => !string.IsNullOrEmpty(x.ProjectData.ShortDescription));

        // ====================================
        // VALIDAÇÃO DA IMAGEM
        // ====================================
        RuleFor(x => x.ProjectData.ImageUrl)
            .NotEmpty().WithMessage("A URL da imagem é obrigatória")
            .MaximumLength(500).WithMessage("A URL da imagem deve ter no máximo 500 caracteres")
            .Must(BeAValidUrl).WithMessage("A URL da imagem deve ser válida");

        // ====================================
        // VALIDAÇÃO DAS URLs (OPCIONAIS)
        // ====================================
        RuleFor(x => x.ProjectData.DemoUrl)
            .MaximumLength(500).WithMessage("A URL da demo deve ter no máximo 500 caracteres")
            .Must(BeAValidUrl).WithMessage("A URL da demo deve ser válida")
            .When(x => !string.IsNullOrEmpty(x.ProjectData.DemoUrl));

        RuleFor(x => x.ProjectData.RepositoryUrl)
            .MaximumLength(500).WithMessage("A URL do repositório deve ter no máximo 500 caracteres")
            .Must(BeAValidUrl).WithMessage("A URL do repositório deve ser válida")
            .When(x => !string.IsNullOrEmpty(x.ProjectData.RepositoryUrl));

        // ====================================
        // VALIDAÇÃO DAS TECNOLOGIAS
        // ====================================
        RuleFor(x => x.ProjectData.Technologies)
            .NotEmpty().WithMessage("Informe pelo menos uma tecnologia utilizada")
            .Must(x => x.Count <= 10).WithMessage("Informe no máximo 10 tecnologias");

        // ====================================
        // VALIDAÇÃO DO ANO
        // ====================================
        RuleFor(x => x.ProjectData.Year)
            .GreaterThan(2000).WithMessage("O ano deve ser maior que 2000")
            .LessThanOrEqualTo(DateTime.Now.Year + 1).WithMessage($"O ano não pode ser maior que {DateTime.Now.Year + 1}");

        // ====================================
        // VALIDAÇÃO DO ENUM STATUS
        // ====================================
        RuleFor(x => x.ProjectData.Status)
            .IsInEnum().WithMessage("Status inválido");
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