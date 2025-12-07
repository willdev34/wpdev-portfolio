// ====================================
// Título: CreateContactMessageCommandValidator.cs
// Descrição: Validações para criação de mensagem de contato
// Autor: Will
// Empresa: WpDev
// Data: 06/12/2024
// ====================================

using FluentValidation;

namespace Portfolio.Application.Commands.ContactMessages.CreateContactMessage;

/// <summary>
/// Validador para CreateContactMessageCommand
/// Define todas as regras de validação para o formulário de contato
/// Se alguma regra falhar, o Command não será processado
/// </summary>
public class CreateContactMessageCommandValidator : AbstractValidator<CreateContactMessageCommand>
{
    public CreateContactMessageCommandValidator()
    {
        // VALIDAÇÃO DO NOME
        RuleFor(x => x.MessageData.Name)
            .NotEmpty().WithMessage("O nome é obrigatório")
            .MinimumLength(2).WithMessage("O nome deve ter no mínimo 2 caracteres")
            .MaximumLength(100).WithMessage("O nome deve ter no máximo 100 caracteres");

        // VALIDAÇÃO DO EMAIL
        RuleFor(x => x.MessageData.Email)
            .NotEmpty().WithMessage("O email é obrigatório")
            .EmailAddress().WithMessage("O email deve ser válido")
            .MaximumLength(200).WithMessage("O email deve ter no máximo 200 caracteres");

        // VALIDAÇÃO DO ASSUNTO
        RuleFor(x => x.MessageData.Subject)
            .NotEmpty().WithMessage("O assunto é obrigatório")
            .MinimumLength(5).WithMessage("O assunto deve ter no mínimo 5 caracteres")
            .MaximumLength(200).WithMessage("O assunto deve ter no máximo 200 caracteres");

        // VALIDAÇÃO DA MENSAGEM
        RuleFor(x => x.MessageData.Message)
            .NotEmpty().WithMessage("A mensagem é obrigatória")
            .MinimumLength(10).WithMessage("A mensagem deve ter no mínimo 10 caracteres")
            .MaximumLength(2000).WithMessage("A mensagem deve ter no máximo 2000 caracteres");

        // VALIDAÇÃO DO TIPO (ENUM)
        RuleFor(x => x.MessageData.Type)
            .GreaterThanOrEqualTo(0).WithMessage("Tipo de mensagem inválido")
            .LessThanOrEqualTo(4).WithMessage("Tipo de mensagem inválido");
    }
}