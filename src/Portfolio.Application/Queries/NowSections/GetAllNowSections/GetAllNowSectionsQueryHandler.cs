// ====================================
// Título: GetAllNowSectionsQueryHandler.cs
// Descrição: Handler que processa GetAllNowSectionsQuery e retorna as seções
// Autor: Will
// Empresa: WpDev
// Data: 06/12/2024
// ====================================

using AutoMapper;
using MediatR;
using Portfolio.Application.DTOs.NowSections;
using Portfolio.Application.Interfaces;

namespace Portfolio.Application.Queries.NowSections.GetAllNowSections;

/// <summary>
/// Handler responsável por processar a query GetAllNowSectionsQuery
/// 1. Busca as seções no banco (via Repository)
/// 2. Converte de Entity para DTO (via AutoMapper)
/// 3. Retorna a lista de NowSectionDto
/// </summary>
public class GetAllNowSectionsQueryHandler : IRequestHandler<GetAllNowSectionsQuery, IEnumerable<NowSectionDto>>
{
    private readonly INowSectionRepository _repository;
    private readonly IMapper _mapper;

    // ====================================
    // CONSTRUTOR - Injeção de Dependência
    // ====================================
    public GetAllNowSectionsQueryHandler(
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
    public async Task<IEnumerable<NowSectionDto>> Handle(
        GetAllNowSectionsQuery request, 
        CancellationToken cancellationToken)
    {
        // 1. Busca TODAS as seções do banco (ordenadas por UpdatedAt desc)
        var nowSections = await _repository.GetAllAsync();

        // 2. Converte de NowSection (Entity) para NowSectionDto
        var nowSectionDtos = _mapper.Map<IEnumerable<NowSectionDto>>(nowSections);

        // 3. Retorna a lista de DTOs
        return nowSectionDtos;
    }
}