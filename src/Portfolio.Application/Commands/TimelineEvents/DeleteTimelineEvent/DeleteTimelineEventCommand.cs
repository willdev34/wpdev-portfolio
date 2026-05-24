// ====================================
// Título: DeleteTimelineEventCommand.cs
// Descrição: Command para deletar um evento (soft delete - CQRS - Write)
// ====================================

using MediatR;

namespace Portfolio.Application.Commands.TimelineEvents.DeleteTimelineEvent;

public class DeleteTimelineEventCommand : IRequest<Unit>
{
    public Guid Id { get; set; }

    public DeleteTimelineEventCommand(Guid id)
    {
        Id = id;
    }
}