// ====================================
// Título: GetAllTimelineEventsQueryHandler.cs
// Descrição: Handler que processa GetAllTimelineEventsQuery e retorna os eventos
// Autor: Will
// Empresa: WpDev
// Data: 29/11/2024
// ====================================

using AutoMapper;
using MediatR;
using Portfolio.Application.DTOs.TimelineEvents;
using Portfolio.Application.Interfaces;

namespace Portfolio.Application.Queries.TimelineEvents.GetAllTimelineEvents;

/// <summary>
/// Handler responsável por processar a query GetAllTimelineEventsQuery
/// 1. Busca os eventos no banco (via Repository)
/// 2. Converte de Entity para DTO (via AutoMapper)
/// 3. Retorna a lista de TimelineEventCardDto
/// </summary>
public class GetAllTimelineEventsQueryHandler : IRequestHandler<GetAllTimelineEventsQuery, IEnumerable<TimelineEventCardDto>>
{
    private readonly ITimelineEventRepository _repository;
    private readonly IMapper _mapper;

    // ====================================
    // CONSTRUTOR - Injeção de Dependência
    // ====================================
    public GetAllTimelineEventsQueryHandler(
        ITimelineEventRepository repository,
        IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    // ====================================
    // HANDLE - Processa a Query
    // ====================================
    /// <summary>
    /// Método principal que executa a lógica da query
    /// </summary>
    public async Task<IEnumerable<TimelineEventCardDto>> Handle(
        GetAllTimelineEventsQuery request, 
        CancellationToken cancellationToken)
    {
        // 1. Busca TODOS os eventos do banco (ordenados por Order)
        var timelineEvents = await _repository.GetAllAsync();

        // 2. Converte de TimelineEvent (Entity) para TimelineEventCardDto
        var timelineEventCards = _mapper.Map<IEnumerable<TimelineEventCardDto>>(timelineEvents);

        // 3. Retorna a lista de DTOs
        return timelineEventCards;
    }
}