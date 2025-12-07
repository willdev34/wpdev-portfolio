// ====================================
// Título: UpdateTimelineEventCommandHandler.cs
// Descrição: Handler que processa UpdateTimelineEventCommand e atualiza o evento
// Autor: Will
// Empresa: WpDev
// Data: 29/11/2024
// ====================================

using AutoMapper;
using MediatR;
using Portfolio.Application.Interfaces;

namespace Portfolio.Application.Commands.TimelineEvents.UpdateTimelineEvent;

public class UpdateTimelineEventCommandHandler : IRequestHandler<UpdateTimelineEventCommand, Unit>
{
    private readonly ITimelineEventRepository _repository;
    private readonly IMapper _mapper;

    public UpdateTimelineEventCommandHandler(
        ITimelineEventRepository repository,
        IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<Unit> Handle(
        UpdateTimelineEventCommand request, 
        CancellationToken cancellationToken)
    {
        var existingEvent = await _repository.GetByIdAsync(request.EventData.Id);

        if (existingEvent == null)
        {
            throw new KeyNotFoundException($"Evento com ID {request.EventData.Id} não encontrado");
        }

        var updatedEvent = _mapper.Map(request.EventData, existingEvent);

        await _repository.UpdateAsync(updatedEvent);
        await _repository.SaveChangesAsync();

        return Unit.Value;
    }
}