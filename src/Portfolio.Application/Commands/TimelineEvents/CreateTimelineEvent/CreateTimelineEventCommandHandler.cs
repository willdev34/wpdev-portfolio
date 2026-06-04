// ====================================
// Título: CreateTimelineEventCommandHandler.cs
// Descrição: Handler que processa CreateTimelineEventCommand e cria o evento no banco
// ====================================

using AutoMapper;
using MediatR;
using Portfolio.Application.DTOs.TimelineEvents;
using Portfolio.Application.Interfaces;
using Portfolio.Domain.Entities;

namespace Portfolio.Application.Commands.TimelineEvents.CreateTimelineEvent;

/// <summary>
/// Handler responsável por processar o command CreateTimelineEventCommand
/// 1. Converte DTO para Entity
/// 2. Salva no banco via Repository
/// 3. Retorna o TimelineEventDto do evento criado
/// </summary>
public class CreateTimelineEventCommandHandler : IRequestHandler<CreateTimelineEventCommand, TimelineEventDto>
{
    private readonly ITimelineEventRepository _repository;
    private readonly IMapper _mapper;

    // ====================================
    // CONSTRUTOR - Injeção de Dependência
    // ====================================
    public CreateTimelineEventCommandHandler(
        ITimelineEventRepository repository,
        IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    // ====================================
    // HANDLE - Processa o Command
    // ====================================
    /// <summary>
    /// Método principal que executa a lógica do command
    /// </summary>
    public async Task<TimelineEventDto> Handle(
        CreateTimelineEventCommand request, 
        CancellationToken cancellationToken)
    {
        // ====================================
        // 1. CONVERTER DTO → ENTITY
        // ====================================
        var timelineEvent = _mapper.Map<TimelineEvent>(request.EventData);

        // ====================================
        // 2. SALVAR NO BANCO
        // ====================================
        var createdEvent = await _repository.AddAsync(timelineEvent);
        await _repository.SaveChangesAsync();

        // ====================================
        // 3. CONVERTER ENTITY → DTO E RETORNAR
        // ====================================
        var timelineEventDto = _mapper.Map<TimelineEventDto>(createdEvent);

        return timelineEventDto;
    }
}