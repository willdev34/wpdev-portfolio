// ====================================
// Título: CreateNowSectionCommandValidator.cs
// Descrição: Validações para criação de seção Now
// Autor: Will
// Empresa: WpDev
// Data: 06/12/2024
// ====================================

using FluentValidation;

namespace Portfolio.Application.Commands.NowSections.CreateNowSection;

/// <summary>
/// Validador para CreateNowSectionCommand
/// Define todas as regras de validação para criação da seção Now
/// Se alguma regra falhar, o Command não será processado
/// </summary>
public class CreateNowSectionCommandValidator : AbstractValidator<CreateNowSectionCommand>
{
    public CreateNowSectionCommandValidator()
    {
        // VALIDAÇÃO DO CONTEÚDO
        RuleFor(x => x.SectionData.Content)
            .NotEmpty().WithMessage("O conteúdo é obrigatório")
            .MinimumLength(10).WithMessage("O conteúdo deve ter no mínimo 10 caracteres")
            .MaximumLength(2000).WithMessage("O conteúdo deve ter no máximo 2000 caracteres");

        // VALIDAÇÃO DO PROJETO ATUAL (OPCIONAL)
        RuleFor(x => x.SectionData.CurrentProject)
            .MaximumLength(200).WithMessage("O nome do projeto deve ter no máximo 200 caracteres")
            .When(x => !string.IsNullOrEmpty(x.SectionData.CurrentProject));

        RuleFor(x => x.SectionData.CurrentProjectUrl)
            .MaximumLength(500).WithMessage("A URL do projeto deve ter no máximo 500 caracteres")
            .Must(BeAValidUrl).WithMessage("A URL do projeto deve ser válida")
            .When(x => !string.IsNullOrEmpty(x.SectionData.CurrentProjectUrl));

        // VALIDAÇÃO DAS TECNOLOGIAS APRENDENDO
        RuleFor(x => x.SectionData.CurrentlyLearning)
            .Must(x => x.Count <= 10).WithMessage("Informe no máximo 10 tecnologias aprendendo")
            .When(x => x.SectionData.CurrentlyLearning != null && x.SectionData.CurrentlyLearning.Any());

        RuleForEach(x => x.SectionData.CurrentlyLearning)
            .MaximumLength(100).WithMessage("Cada tecnologia deve ter no máximo 100 caracteres")
            .When(x => x.SectionData.CurrentlyLearning != null && x.SectionData.CurrentlyLearning.Any());

        // VALIDAÇÃO DOS OBJETIVOS
        RuleFor(x => x.SectionData.CurrentGoals)
            .Must(x => x.Count <= 10).WithMessage("Informe no máximo 10 objetivos")
            .When(x => x.SectionData.CurrentGoals != null && x.SectionData.CurrentGoals.Any());

        RuleForEach(x => x.SectionData.CurrentGoals)
            .MaximumLength(200).WithMessage("Cada objetivo deve ter no máximo 200 caracteres")
            .When(x => x.SectionData.CurrentGoals != null && x.SectionData.CurrentGoals.Any());
    }

    private bool BeAValidUrl(string? url)
    {
        if (string.IsNullOrEmpty(url))
            return true;

        return Uri.TryCreate(url, UriKind.Absolute, out var uriResult)
               && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
    }
}