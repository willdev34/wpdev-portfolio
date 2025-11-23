// ====================================
// Título: CreateProjectCommandHandler
// Descrição: Handler que processa CreateProjectCommand e cria o projeto no banco
// Autor: Will
// Empresa: WpDev
// Data: 23/11/2024
// ====================================

using AutoMapper;
using MediatR;
using Portfolio.Application.DTOs.Projects;
using Portfolio.Application.Interfaces;
using Portfolio.Domain.Entities;

namespace Portfolio.Application.Commands.Projects.CreateProject;

/// <summary>
/// Handler responsável por processar o command CreateProjectCommand
/// 1. Recebe os dados do DTO
/// 2. Converte para Entity
/// 3. Salva no banco via Repository
/// 4. Retorna o ProjectDto do projeto criado
/// </summary>
public class CreateProjectCommandHandler : IRequestHandler<CreateProjectCommand, ProjectDto>
{
    private readonly IProjectRepository _repository;
    private readonly IMapper _mapper;

    // ====================================
    // CONSTRUTOR - Injeção de Dependência
    // ====================================
    public CreateProjectCommandHandler(
        IProjectRepository repository,
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
    public async Task<ProjectDto> Handle(
        CreateProjectCommand request, 
        CancellationToken cancellationToken)
    {
        // ====================================
        // 1. CONVERTER DTO → ENTITY
        // ====================================
        // O AutoMapper usa o Profile que criamos para fazer a conversão
        var project = _mapper.Map<Project>(request.ProjectData);

        // ====================================
        // 2. SALVAR NO BANCO
        // ====================================
        // O Repository cuida de:
        // - Gerar o ID
        // - Setar CreatedAt
        // - Marcar como IsActive = true
        var createdProject = await _repository.AddAsync(project);
        
        // Salva as mudanças no banco
        await _repository.SaveChangesAsync();

        // ====================================
        // 3. CONVERTER ENTITY → DTO E RETORNAR
        // ====================================
        // Retorna o projeto criado como ProjectDto (com Id gerado)
        var projectDto = _mapper.Map<ProjectDto>(createdProject);

        return projectDto;
    }
}