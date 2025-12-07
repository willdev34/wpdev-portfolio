// ====================================
// Título: UpdateContactMessageCommandHandler.cs
// Descrição: Handler que processa UpdateContactMessageCommand e atualiza a mensagem
// Autor: Will
// Empresa: WpDev
// Data: 06/12/2024
// ====================================

using AutoMapper;
using MediatR;
using Portfolio.Application.Interfaces;
using Portfolio.Domain.Entities;

namespace Portfolio.Application.Commands.ContactMessages.UpdateContactMessage;

public class UpdateContactMessageCommandHandler : IRequestHandler<UpdateContactMessageCommand, Unit>
{
    private readonly IContactMessageRepository _repository;
    private readonly IMapper _mapper;

    public UpdateContactMessageCommandHandler(
        IContactMessageRepository repository,
        IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<Unit> Handle(
        UpdateContactMessageCommand request, 
        CancellationToken cancellationToken)
    {
        // ====================================
        // 1. VERIFICAR SE A MENSAGEM EXISTE
        // ====================================
        var existingMessage = await _repository.GetByIdAsync(request.MessageData.Id);

        if (existingMessage == null)
        {
            throw new KeyNotFoundException($"Mensagem com ID {request.MessageData.Id} não encontrada");
        }

        // ====================================
        // 2. ATUALIZAR STATUS
        // ====================================
        existingMessage.Status = (ContactMessageStatus)request.MessageData.Status;
        
        // ====================================
        // 3. ATUALIZAR RESPOSTA (se houver)
        // ====================================
        if (!string.IsNullOrEmpty(request.MessageData.ResponseMessage))
        {
            existingMessage.ResponseMessage = request.MessageData.ResponseMessage;
            
            // Se está adicionando resposta pela primeira vez, seta RespondedAt
            if (existingMessage.RespondedAt == null)
            {
                existingMessage.RespondedAt = DateTime.UtcNow;
            }
            
            // Muda status para Responded automaticamente
            existingMessage.Status = ContactMessageStatus.Responded;
        }

        // ====================================
        // 4. SALVAR NO BANCO
        // ====================================
        await _repository.UpdateAsync(existingMessage);
        await _repository.SaveChangesAsync();

        return Unit.Value;
    }
}