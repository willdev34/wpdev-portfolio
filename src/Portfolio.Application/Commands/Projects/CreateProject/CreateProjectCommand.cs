// ====================================
// Título: CreateProjectCommand
// Descrição: Command para criar um novo projeto (CQRS - Write)
// Autor: Will
// Empresa: WpDev
// Data: 23/11/2024
// ====================================

using MediatR;
using Portfolio.Application.DTOs.Projects;

namespace Portfolio.Application.Commands.Projects.CreateProject;

/// <summary>
/// Command para criar um novo projeto
/// Recebe os dados do CreateProjectDto e retorna o ProjectDto completo do projeto criado
/// </summary>
public class CreateProjectCommand : IRequest<ProjectDto>
{
    // ====================================
    // DADOS DO PROJETO A SER CRIADO
    // ====================================
    public CreateProjectDto ProjectData { get; set; } = null!;

    // ====================================
    // CONSTRUTOR
    // ====================================
    public CreateProjectCommand(CreateProjectDto projectData)
    {
        ProjectData = projectData;
    }
}