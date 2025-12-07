// ====================================
// Título: GetContactMessageByIdQueryHandler.cs
// Descrição: Handler que processa GetContactMessageByIdQuery e retorna a mensagem
// Autor: Will
// Empresa: WpDev
// Data: 06/12/2024
// ====================================

using AutoMapper;
using MediatR;
using Portfolio.Application.DTOs.ContactMessages;
using Portfolio.Application.Interfaces;

namespace Portfolio.Application.Queries.ContactMessages.GetContactMessageById;

/// <summary>
/// Handler responsável por processar a query GetContactMessageByIdQuery
/// 1. Busca a mensagem no banco por ID (via Repository)
/// 2. Converte de Entity para DTO (via AutoMapper)
/// 3. Retorna o ContactMessageDto ou null se não encontrado
/// </summary>
public class GetContactMessageByIdQueryHandler : IRequestHandler<GetContactMessageByIdQuery, ContactMessageDto?>
{
    private readonly IContactMessageRepository _repository;
    private readonly IMapper _mapper;

    // ====================================
    // CONSTRUTOR - Injeção de Dependência
    // ====================================
    public GetContactMessageByIdQueryHandler(
        IContactMessageRepository repository,
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
    public async Task<ContactMessageDto?> Handle(
        GetContactMessageByIdQuery request, 
        CancellationToken cancellationToken)
    {
        // 1. Busca a mensagem no banco por ID
        var contactMessage = await _repository.GetByIdAsync(request.Id);

        // 2. Se não encontrou, retorna null
        if (contactMessage == null)
        {
            return null;
        }

        // 3. Converte de ContactMessage (Entity) para ContactMessageDto
        var contactMessageDto = _mapper.Map<ContactMessageDto>(contactMessage);

        // 4. Retorna o DTO
        return contactMessageDto;
    }
}