// Título: DeleteContactMessageCommand.cs
// Descrição: Command para deletar uma mensagem de contato (hard delete - CQRS - Write)

using MediatR;

namespace Portfolio.Application.Commands.ContactMessages.DeleteContactMessage;

/// <summary>
/// Command para deletar uma mensagem de contato fisicamente do banco
/// Usado pelo admin quando decide descartar definitivamente uma mensagem
/// Retorna Unit (void)
/// Usado no endpoint DELETE /api/contactmessages/{id} (ADMIN)
/// </summary>
public class DeleteContactMessageCommand : IRequest<Unit>
{
    public Guid Id { get; set; }

    public DeleteContactMessageCommand(Guid id)
    {
        Id = id;
    }
}