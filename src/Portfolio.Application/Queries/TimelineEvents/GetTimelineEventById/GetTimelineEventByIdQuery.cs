// ====================================
// Título: GetTimelineEventByIdQuery.cs
// Descrição: Query para buscar um evento específico por ID (CQRS - Read)
// Autor: Will
// Empresa: WpDev
// Data: 29/11/2024
// ====================================

using MediatR;
using Portfolio.Application.DTOs.TimelineEvents;

namespace Portfolio.Application.Queries.TimelineEvents.GetTimelineEventById;

/// <summary>
/// Query para buscar um evento específico por ID
/// Retorna TimelineEventDto completo ou null se não encontrado
/// Usado no endpoint GET /api/timelineevents/{id}
/// </summary>
public class GetTimelineEventByIdQuery : IRequest<TimelineEventDto?>
{
    // ====================================
    // PARÂMETRO DA QUERY
    // ====================================
    public Guid Id { get; set; }

    // ====================================
    // CONSTRUTOR
    // ====================================
    public GetTimelineEventByIdQuery(Guid id)
    {
        Id = id;
    }
}