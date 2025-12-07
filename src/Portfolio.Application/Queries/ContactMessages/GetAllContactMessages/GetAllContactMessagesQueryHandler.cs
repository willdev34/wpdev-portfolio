// ====================================
// Título: GetAllContactMessagesQueryHandler.cs
// Descrição: Handler que processa GetAllContactMessagesQuery e retorna as mensagens
// Autor: Will
// Empresa: WpDev
// Data: 06/12/2024
// ====================================

using AutoMapper;
using MediatR;
using Portfolio.Application.DTOs.ContactMessages;
using Portfolio.Application.Interfaces;

namespace Portfolio.Application.Queries.ContactMessages.GetAllContactMessages;

/// <summary>
/// Handler responsável por processar a query GetAllContactMessagesQuery
/// 1. Busca as mensagens no banco (via Repository)
/// 2. Converte de Entity para DTO (via AutoMapper)
/// 3. Retorna a lista de ContactMessageCardDto
/// </summary>
public class GetAllContactMessagesQueryHandler : IRequestHandler<GetAllContactMessagesQuery, IEnumerable<ContactMessageCardDto>>
{
    private readonly IContactMessageRepository _repository;
    private readonly IMapper _mapper;

    // ====================================
    // CONSTRUTOR - Injeção de Dependência
    // ====================================
    public GetAllContactMessagesQueryHandler(
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
    public async Task<IEnumerable<ContactMessageCardDto>> Handle(
        GetAllContactMessagesQuery request, 
        CancellationToken cancellationToken)
    {
        // 1. Busca TODAS as mensagens do banco (ordenadas por CreatedAt desc)
        var contactMessages = await _repository.GetAllAsync();

        // 2. Converte de ContactMessage (Entity) para ContactMessageCardDto
        var contactMessageCards = _mapper.Map<IEnumerable<ContactMessageCardDto>>(contactMessages);

        // 3. Retorna a lista de DTOs
        return contactMessageCards;
    }
}