// ====================================
// Título: DeleteTimelineEventCommandHandler.cs
// Descrição: Handler que processa DeleteTimelineEventCommand e marca evento como invisível
// Autor: Will
// Empresa: WpDev
// Data: 29/11/2024
// ====================================

using MediatR;
using Portfolio.Application.Interfaces;

namespace Portfolio.Application.Commands.TimelineEvents.DeleteTimelineEvent;

public class DeleteTimelineEventCommandHandler : IRequestHandler<DeleteTimelineEventCommand, Unit>
{
    private readonly ITimelineEventRepository _repository;

    public DeleteTimelineEventCommandHandler(ITimelineEventRepository repository)
    {
        _repository = repository;
    }

    public async Task<Unit> Handle(
        DeleteTimelineEventCommand request, 
        CancellationToken cancellationToken)
    {
        var exists = await _repository.ExistsAsync(request.Id);

        if (!exists)
        {
            throw new KeyNotFoundException($"Evento com ID {request.Id} não encontrado");
        }

        await _repository.DeleteAsync(request.Id);
        await _repository.SaveChangesAsync();

        return Unit.Value;
    }
}