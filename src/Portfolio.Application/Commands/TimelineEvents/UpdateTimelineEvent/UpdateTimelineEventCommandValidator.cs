// ====================================
// Título: UpdateTimelineEventCommandValidator.cs
// Descrição: Validações para atualização de evento da timeline
// Autor: Will
// Empresa: WpDev
// Data: 29/11/2024
// ====================================

using FluentValidation;

namespace Portfolio.Application.Commands.TimelineEvents.UpdateTimelineEvent;

public class UpdateTimelineEventCommandValidator : AbstractValidator<UpdateTimelineEventCommand>
{
    public UpdateTimelineEventCommandValidator()
    {
        RuleFor(x => x.EventData.Id)
            .NotEmpty().WithMessage("O ID do evento é obrigatório");

        RuleFor(x => x.EventData.Title)
            .NotEmpty().WithMessage("O título do evento é obrigatório")
            .MinimumLength(3).WithMessage("O título deve ter no mínimo 3 caracteres")
            .MaximumLength(200).WithMessage("O título deve ter no máximo 200 caracteres");

        RuleFor(x => x.EventData.Description)
            .NotEmpty().WithMessage("A descrição do evento é obrigatória")
            .MinimumLength(10).WithMessage("A descrição deve ter no mínimo 10 caracteres")
            .MaximumLength(1000).WithMessage("A descrição deve ter no máximo 1000 caracteres");

        RuleFor(x => x.EventData.Date)
            .NotEmpty().WithMessage("A data do evento é obrigatória")
            .LessThanOrEqualTo(DateTime.Now.AddYears(1))
                .WithMessage("A data não pode ser maior que 1 ano no futuro");

        RuleFor(x => x.EventData.Type)
            .GreaterThanOrEqualTo(0).WithMessage("Tipo de evento inválido")
            .LessThanOrEqualTo(5).WithMessage("Tipo de evento inválido");

        RuleFor(x => x.EventData.IconUrl)
            .MaximumLength(500).WithMessage("A URL do ícone deve ter no máximo 500 caracteres")
            .Must(BeAValidUrl).WithMessage("A URL do ícone deve ser válida")
            .When(x => !string.IsNullOrEmpty(x.EventData.IconUrl));

        RuleFor(x => x.EventData.LinkUrl)
            .MaximumLength(500).WithMessage("A URL do link deve ter no máximo 500 caracteres")
            .Must(BeAValidUrl).WithMessage("A URL do link deve ser válida")
            .When(x => !string.IsNullOrEmpty(x.EventData.LinkUrl));

        RuleFor(x => x.EventData.LinkText)
            .MaximumLength(100).WithMessage("O texto do link deve ter no máximo 100 caracteres")
            .When(x => !string.IsNullOrEmpty(x.EventData.LinkText));

        RuleFor(x => x.EventData.Order)
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