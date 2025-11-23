// ====================================
// Título: GetAllProjectsQuery
// Descrição: Query para buscar todos os projetos (CQRS - Read)
// Autor: Will
// Empresa: WpDev
// Data: 23/11/2024
// ====================================

using MediatR;
using Portfolio.Application.DTOs.Projects;

namespace Portfolio.Application.Queries.Projects.GetAllProjects;

/// <summary>
/// Query para buscar TODOS os projetos ativos
/// Retorna uma lista de ProjectCardDto (versão simplificada para grid)
/// </summary>
public class GetAllProjectsQuery : IRequest<IEnumerable<ProjectCardDto>>
{
    // Esta query não precisa de parâmetros
    // Ela simplesmente retorna TODOS os projetos ativos
}