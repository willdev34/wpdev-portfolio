// ====================================
// Título: UpdateContactMessageCommand.cs
// Descrição: Command para atualizar uma mensagem existente (CQRS - Write)
// Autor: Will
// Empresa: WpDev
// Data: 06/12/2024
// ====================================

using MediatR;
using Portfolio.Application.DTOs.ContactMessages;

namespace Portfolio.Application.Commands.ContactMessages.UpdateContactMessage;

/// <summary>
/// Command para atualizar uma mensagem existente
/// Usado pelo ADMIN para mudar status, adicionar resposta
/// Recebe os dados do UpdateContactMessageDto
/// Retorna Unit (void) - não retorna nada em caso de sucesso
/// Usado no endpoint PUT /api/contactmessages/{id} (ADMIN)
/// </summary>
public class UpdateContactMessageCommand : IRequest<Unit>
{
    public UpdateContactMessageDto MessageData { get; set; } = null!;

    public UpdateContactMessageCommand(UpdateContactMessageDto messageData)
    {
        MessageData = messageData;
    }
}