// ====================================
// Título: CreateTimelineEventCommand.cs
// Descrição: Command para criar um novo evento da timeline (CQRS - Write)
// Autor: Will
// Empresa: WpDev
// Data: 29/11/2024
// ====================================

using MediatR;
using Portfolio.Application.DTOs.TimelineEvents;

namespace Portfolio.Application.Commands.TimelineEvents.CreateTimelineEvent;

/// <summary>
/// Command para criar um novo evento da timeline
/// Recebe os dados do CreateTimelineEventDto e retorna o TimelineEventDto completo do evento criado
/// Usado no endpoint POST /api/timelineevents
/// </summary>
public class CreateTimelineEventCommand : IRequest<TimelineEventDto>
{
    // ====================================
    // DADOS DO EVENTO A SER CRIADO
    // ====================================
    public CreateTimelineEventDto EventData { get; set; } = null!;

    // ====================================
    // CONSTRUTOR
    // ====================================
    public CreateTimelineEventCommand(CreateTimelineEventDto eventData)
    {
        EventData = eventData;
    }
}