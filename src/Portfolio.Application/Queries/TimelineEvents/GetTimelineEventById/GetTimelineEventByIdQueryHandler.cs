// ====================================
// Título: GetTimelineEventByIdQueryHandler.cs
// Descrição: Handler que processa GetTimelineEventByIdQuery e retorna o evento
// Autor: Will
// Empresa: WpDev
// Data: 29/11/2024
// ====================================

using AutoMapper;
using MediatR;
using Portfolio.Application.DTOs.TimelineEvents;
using Portfolio.Application.Interfaces;

namespace Portfolio.Application.Queries.TimelineEvents.GetTimelineEventById;

/// <summary>
/// Handler responsável por processar a query GetTimelineEventByIdQuery
/// 1. Busca o evento no banco por ID (via Repository)
/// 2. Converte de Entity para DTO (via AutoMapper)
/// 3. Retorna o TimelineEventDto ou null se não encontrado
/// </summary>
public class GetTimelineEventByIdQueryHandler : IRequestHandler<GetTimelineEventByIdQuery, TimelineEventDto?>
{
    private readonly ITimelineEventRepository _repository;
    private readonly IMapper _mapper;

    // ====================================
    // CONSTRUTOR - Injeção de Dependência
    // ====================================
    public GetTimelineEventByIdQueryHandler(
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
    public async Task<TimelineEventDto?> Handle(
        GetTimelineEventByIdQuery request, 
        CancellationToken cancellationToken)
    {
        // 1. Busca o evento no banco por ID
        var timelineEvent = await _repository.GetByIdAsync(request.Id);

        // 2. Se não encontrou, retorna null
        if (timelineEvent == null)
        {
            return null;
        }

        // 3. Converte de TimelineEvent (Entity) para TimelineEventDto
        var timelineEventDto = _mapper.Map<TimelineEventDto>(timelineEvent);

        // 4. Retorna o DTO
        return timelineEventDto;
    }
}