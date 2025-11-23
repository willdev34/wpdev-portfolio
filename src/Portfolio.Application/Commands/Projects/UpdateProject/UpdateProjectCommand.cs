// ====================================
// Título: UpdateProjectCommand
// Descrição: Command para atualizar um projeto existente (CQRS - Write)
// Autor: Will
// Empresa: WpDev
// Data: 23/11/2024
// ====================================

using MediatR;
using Portfolio.Application.DTOs.Projects;

namespace Portfolio.Application.Commands.Projects.UpdateProject;

/// <summary>
/// Command para atualizar um projeto existente
/// Recebe os dados do UpdateProjectDto
/// Retorna Unit (void) - não retorna nada em caso de sucesso
/// </summary>
public class UpdateProjectCommand : IRequest<Unit>
{
    // ====================================
    // DADOS DO PROJETO A SER ATUALIZADO
    // ====================================
    public UpdateProjectDto ProjectData { get; set; } = null!;

    // ====================================
    // CONSTRUTOR
    // ====================================
    public UpdateProjectCommand(UpdateProjectDto projectData)
    {
        ProjectData = projectData;
    }
}