// ====================================
// Título: GetAllProjectsQueryHandler
// Descrição: Handler que processa GetAllProjectsQuery e retorna os projetos
// Autor: Will
// Empresa: WpDev
// Data: 23/11/2024
// ====================================

using AutoMapper;
using MediatR;
using Portfolio.Application.DTOs.Projects;
using Portfolio.Application.Interfaces;

namespace Portfolio.Application.Queries.Projects.GetAllProjects;

/// <summary>
/// Handler responsável por processar a query GetAllProjectsQuery
/// 1. Busca os projetos no banco (via Repository)
/// 2. Converte de Entity para DTO (via AutoMapper)
/// 3. Retorna a lista de ProjectCardDto
/// </summary>
public class GetAllProjectsQueryHandler : IRequestHandler<GetAllProjectsQuery, IEnumerable<ProjectCardDto>>
{
    private readonly IProjectRepository _repository;
    private readonly IMapper _mapper;

    // ====================================
    // CONSTRUTOR - Injeção de Dependência
    // ====================================
    // O ASP.NET injeta automaticamente Repository e Mapper
    public GetAllProjectsQueryHandler(
        IProjectRepository repository,
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
    public async Task<IEnumerable<ProjectCardDto>> Handle(
        GetAllProjectsQuery request, 
        CancellationToken cancellationToken)
    {
        // 1. Busca TODOS os projetos ativos do banco
        var projects = await _repository.GetAllAsync();

        // 2. Converte de Project (Entity) para ProjectCardDto
        // O AutoMapper faz isso automaticamente usando o Profile que criamos
        var projectCards = _mapper.Map<IEnumerable<ProjectCardDto>>(projects);

        // 3. Retorna a lista de DTOs
        return projectCards;
    }
}