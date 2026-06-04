// ====================================
// Título: GetProjectByIdQuery
// Descrição: Query para buscar um projeto específico por ID (CQRS - Read)
// ====================================

using MediatR;
using Portfolio.Application.DTOs.Projects;

namespace Portfolio.Application.Queries.Projects.GetProjectById;

/// <summary>
/// Query para buscar um projeto específico por ID
/// Retorna ProjectDto completo ou null se não encontrado
/// </summary>
public class GetProjectByIdQuery : IRequest<ProjectDto?>
{
    // ====================================
    // PARÂMETRO DA QUERY
    // ====================================
    public Guid Id { get; set; }

    // ====================================
    // CONSTRUTOR
    // ====================================
    public GetProjectByIdQuery(Guid id)
    {
        Id = id;
    }
}