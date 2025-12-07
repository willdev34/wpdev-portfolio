// ====================================
// Título: GetNowSectionByIdQueryHandler.cs
// Descrição: Handler que processa GetNowSectionByIdQuery e retorna a seção
// Autor: Will
// Empresa: WpDev
// Data: 06/12/2024
// ====================================

using AutoMapper;
using MediatR;
using Portfolio.Application.DTOs.NowSections;
using Portfolio.Application.Interfaces;

namespace Portfolio.Application.Queries.NowSections.GetNowSectionById;

/// <summary>
/// Handler responsável por processar a query GetNowSectionByIdQuery
/// 1. Busca a seção no banco por ID (via Repository)
/// 2. Converte de Entity para DTO (via AutoMapper)
/// 3. Retorna o NowSectionDto ou null se não encontrado
/// </summary>
public class GetNowSectionByIdQueryHandler : IRequestHandler<GetNowSectionByIdQuery, NowSectionDto?>
{
    private readonly INowSectionRepository _repository;
    private readonly IMapper _mapper;

    // ====================================
    // CONSTRUTOR - Injeção de Dependência
    // ====================================
    public GetNowSectionByIdQueryHandler(
        INowSectionRepository repository,
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
    public async Task<NowSectionDto?> Handle(
        GetNowSectionByIdQuery request, 
        CancellationToken cancellationToken)
    {
        // 1. Busca a seção no banco por ID
        var nowSection = await _repository.GetByIdAsync(request.Id);

        // 2. Se não encontrou, retorna null
        if (nowSection == null)
        {
            return null;
        }

        // 3. Converte de NowSection (Entity) para NowSectionDto
        var nowSectionDto = _mapper.Map<NowSectionDto>(nowSection);

        // 4. Retorna o DTO
        return nowSectionDto;
    }
}