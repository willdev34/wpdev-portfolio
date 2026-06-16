// Título: DeleteContactMessageCommandHandler.cs
// Descrição: Handler que processa DeleteContactMessageCommand e deleta a mensagem fisicamente

using MediatR;
using Portfolio.Application.Interfaces;

namespace Portfolio.Application.Commands.ContactMessages.DeleteContactMessage;

/// <summary>
/// Handler responsável por processar o command DeleteContactMessageCommand
/// Implementa hard delete (deleção física do banco)
/// </summary>
public class DeleteContactMessageCommandHandler : IRequestHandler<DeleteContactMessageCommand, Unit>
{
    private readonly IContactMessageRepository _repository;

    public DeleteContactMessageCommandHandler(IContactMessageRepository repository)
    {
        _repository = repository;
    }

    public async Task<Unit> Handle(
        DeleteContactMessageCommand request,
        CancellationToken cancellationToken)
    {
        // 1. Verificar se a mensagem existe
        var exists = await _repository.ExistsAsync(request.Id);

        if (!exists)
        {
            throw new KeyNotFoundException($"Mensagem com ID {request.Id} não encontrada");
        }

        // 2. Hard delete
        await _repository.DeleteAsync(request.Id);
        await _repository.SaveChangesAsync();

        return Unit.Value;
    }
}