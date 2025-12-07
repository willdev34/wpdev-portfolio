// ====================================
// Título: GetAllTimelineEventsQuery.cs
// Descrição: Query para buscar todos os eventos da timeline (CQRS - Read)
// Autor: Will
// Empresa: WpDev
// Data: 29/11/2024
// ====================================

using MediatR;
using Portfolio.Application.DTOs.TimelineEvents;

namespace Portfolio.Application.Queries.TimelineEvents.GetAllTimelineEvents;

/// <summary>
/// Query para buscar TODOS os eventos da timeline
/// Retorna uma lista de TimelineEventCardDto (versão simplificada)
/// Ordenados por Order (crescente)
/// Usado no endpoint GET /api/timelineevents
/// </summary>
public class GetAllTimelineEventsQuery : IRequest<IEnumerable<TimelineEventCardDto>>
{
    // Esta query não precisa de parâmetros
    // Ela simplesmente retorna TODOS os eventos
}