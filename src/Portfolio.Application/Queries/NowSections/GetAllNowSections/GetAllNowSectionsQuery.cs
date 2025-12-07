// ====================================
// Título: GetAllNowSectionsQuery.cs
// Descrição: Query para buscar todas as seções Now (CQRS - Read)
// Autor: Will
// Empresa: WpDev
// Data: 06/12/2024
// ====================================

using MediatR;
using Portfolio.Application.DTOs.NowSections;

namespace Portfolio.Application.Queries.NowSections.GetAllNowSections;

/// <summary>
/// Query para buscar TODAS as seções Now
/// Retorna uma lista de NowSectionDto
/// Ordenadas por UpdatedAt (mais recente primeiro)
/// Usado no endpoint GET /api/nowsections (ADMIN)
/// </summary>
public class GetAllNowSectionsQuery : IRequest<IEnumerable<NowSectionDto>>
{
    // Esta query não precisa de parâmetros
    // Ela simplesmente retorna TODAS as seções
}