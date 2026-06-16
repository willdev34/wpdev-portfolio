// ====================================
// Título: CreateNowSectionCommandValidator.cs
// Descrição: Validações para criação de seção Now
// ====================================

using FluentValidation;

namespace Portfolio.Application.Commands.NowSections.CreateNowSection;

public class CreateNowSectionCommandValidator : AbstractValidator<CreateNowSectionCommand>
{
    public CreateNowSectionCommandValidator()
    {
        RuleFor(x => x.SectionData.Content)
            .NotEmpty().WithMessage("O conteúdo é obrigatório")
            .MinimumLength(10).WithMessage("O conteúdo deve ter no mínimo 10 caracteres")
            .MaximumLength(2000).WithMessage("O conteúdo deve ter no máximo 2000 caracteres");

        RuleFor(x => x.SectionData.CurrentProjects)
            .Must(x => x.Count <= 10).WithMessage("Informe no máximo 10 projetos")
            .When(x => x.SectionData.CurrentProjects != null && x.SectionData.CurrentProjects.Any());

        RuleForEach(x => x.SectionData.CurrentProjects)
            .Must(p => !string.IsNullOrWhiteSpace(p.Name)).WithMessage("O nome do projeto é obrigatório")
            .Must(p => p.Name.Length <= 200).WithMessage("O nome do projeto deve ter no máximo 200 caracteres")
            .Must(p => string.IsNullOrEmpty(p.Url) || Uri.TryCreate(p.Url, UriKind.Absolute, out _))
                .WithMessage("A URL do projeto deve ser válida")
            .When(x => x.SectionData.CurrentProjects != null && x.SectionData.CurrentProjects.Any());

        RuleFor(x => x.SectionData.CurrentlyLearning)
            .Must(x => x.Count <= 10).WithMessage("Informe no máximo 10 tecnologias aprendendo")
            .When(x => x.SectionData.CurrentlyLearning != null && x.SectionData.CurrentlyLearning.Any());

        RuleForEach(x => x.SectionData.CurrentlyLearning)
            .MaximumLength(100).WithMessage("Cada tecnologia deve ter no máximo 100 caracteres")
            .When(x => x.SectionData.CurrentlyLearning != null && x.SectionData.CurrentlyLearning.Any());

        RuleFor(x => x.SectionData.CurrentGoals)
            .Must(x => x.Count <= 10).WithMessage("Informe no máximo 10 objetivos")
            .When(x => x.SectionData.CurrentGoals != null && x.SectionData.CurrentGoals.Any());

        RuleForEach(x => x.SectionData.CurrentGoals)
            .MaximumLength(200).WithMessage("Cada objetivo deve ter no máximo 200 caracteres")
            .When(x => x.SectionData.CurrentGoals != null && x.SectionData.CurrentGoals.Any());
    }
}