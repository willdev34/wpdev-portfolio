// ====================================
// Título: UpdateContactMessageCommandValidator.cs
// Descrição: Validações para atualização de mensagem de contato (ADMIN)
// Autor: Will
// Empresa: WpDev
// Data: 06/12/2024
// ====================================

using FluentValidation;

namespace Portfolio.Application.Commands.ContactMessages.UpdateContactMessage;

public class UpdateContactMessageCommandValidator : AbstractValidator<UpdateContactMessageCommand>
{
    public UpdateContactMessageCommandValidator()
    {
        // VALIDAÇÃO DO ID
        RuleFor(x => x.MessageData.Id)
            .NotEmpty().WithMessage("O ID da mensagem é obrigatório");

        // VALIDAÇÃO DO STATUS (ENUM)
        RuleFor(x => x.MessageData.Status)
            .GreaterThanOrEqualTo(0).WithMessage("Status inválido")
            .LessThanOrEqualTo(4).WithMessage("Status inválido");

        // VALIDAÇÃO DA RESPOSTA (opcional)
        RuleFor(x => x.MessageData.ResponseMessage)
            .MaximumLength(2000).WithMessage("A resposta deve ter no máximo 2000 caracteres")
            .When(x => !string.IsNullOrEmpty(x.MessageData.ResponseMessage));
    }
}