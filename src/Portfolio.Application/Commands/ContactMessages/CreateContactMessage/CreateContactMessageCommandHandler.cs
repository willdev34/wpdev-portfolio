// ====================================
// Título: CreateContactMessageCommandHandler.cs
// Descrição: Handler que processa CreateContactMessageCommand e cria a mensagem no banco
// Autor: Will
// Empresa: WpDev
// Data: 06/12/2024
// ====================================

using AutoMapper;
using MediatR;
using Portfolio.Application.DTOs.ContactMessages;
using Portfolio.Application.Interfaces;
using Portfolio.Domain.Entities;

namespace Portfolio.Application.Commands.ContactMessages.CreateContactMessage;

/// <summary>
/// Handler responsável por processar o command CreateContactMessageCommand
/// 1. Converte DTO para Entity
/// 2. Adiciona IP e UserAgent (segurança)
/// 3. Salva no banco via Repository
/// 4. Retorna o ContactMessageDto da mensagem criada
/// </summary>
public class CreateContactMessageCommandHandler : IRequestHandler<CreateContactMessageCommand, ContactMessageDto>
{
    private readonly IContactMessageRepository _repository;
    private readonly IMapper _mapper;

    // ====================================
    // CONSTRUTOR - Injeção de Dependência
    // ====================================
    public CreateContactMessageCommandHandler(
        IContactMessageRepository repository,
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
    public async Task<ContactMessageDto> Handle(
        CreateContactMessageCommand request, 
        CancellationToken cancellationToken)
    {
        // ====================================
        // 1. CONVERTER DTO → ENTITY
        // ====================================
        var contactMessage = _mapper.Map<ContactMessage>(request.MessageData);
        
        // ====================================
        // 2. ADICIONAR DADOS DE SEGURANÇA
        // ====================================
        contactMessage.IpAddress = request.IpAddress;
        contactMessage.UserAgent = request.UserAgent;

        // ====================================
        // 3. SALVAR NO BANCO
        // ====================================
        var createdMessage = await _repository.AddAsync(contactMessage);
        await _repository.SaveChangesAsync();

        // ====================================
        // 4. CONVERTER ENTITY → DTO E RETORNAR
        // ====================================
        var contactMessageDto = _mapper.Map<ContactMessageDto>(createdMessage);

        return contactMessageDto;
    }
}