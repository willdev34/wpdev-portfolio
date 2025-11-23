// ====================================
// Título: GetProjectByIdQueryHandler
// Descrição: Handler que processa GetProjectByIdQuery e retorna o projeto
// Autor: Will
// Empresa: WpDev
// Data: 23/11/2024
// ====================================

using AutoMapper;
using MediatR;
using Portfolio.Application.DTOs.Projects;
using Portfolio.Application.Interfaces;

namespace Portfolio.Application.Queries.Projects.GetProjectById;

/// <summary>
/// Handler responsável por processar a query GetProjectByIdQuery
/// 1. Busca o projeto no banco por ID (via Repository)
/// 2. Converte de Entity para DTO (via AutoMapper)
/// 3. Retorna o ProjectDto ou null se não encontrado
/// </summary>
public class GetProjectByIdQueryHandler : IRequestHandler<GetProjectByIdQuery, ProjectDto?>
{
    private readonly IProjectRepository _repository;
    private readonly IMapper _mapper;

    // ====================================
    // CONSTRUTOR - Injeção de Dependência
    // ====================================
    public GetProjectByIdQueryHandler(
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
    public async Task<ProjectDto?> Handle(
        GetProjectByIdQuery request, 
        CancellationToken cancellationToken)
    {
        // 1. Busca o projeto no banco por ID
        var project = await _repository.GetByIdAsync(request.Id);

        // 2. Se não encontrou, retorna null
        if (project == null)
        {
            return null;
        }

        // 3. Converte de Project (Entity) para ProjectDto
        var projectDto = _mapper.Map<ProjectDto>(project);

        // 4. Retorna o DTO
        return projectDto;
    }
}