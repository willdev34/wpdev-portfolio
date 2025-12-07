// ====================================
// Título: UpdateTimelineEventCommand.cs
// Descrição: Command para atualizar um evento existente (CQRS - Write)
// Autor: Will
// Empresa: WpDev
// Data: 29/11/2024
// ====================================

using MediatR;
using Portfolio.Application.DTOs.TimelineEvents;

namespace Portfolio.Application.Commands.TimelineEvents.UpdateTimelineEvent;

/// <summary>
/// Command para atualizar um evento existente
/// Recebe os dados do UpdateTimelineEventDto
/// Retorna Unit (void) - não retorna nada em caso de sucesso
/// Usado no endpoint PUT /api/timelineevents/{id}
/// </summary>
public class UpdateTimelineEventCommand : IRequest<Unit>
{
    public UpdateTimelineEventDto EventData { get; set; } = null!;

    public UpdateTimelineEventCommand(UpdateTimelineEventDto eventData)
    {
        EventData = eventData;
    }
}