// ====================================
// Título: GetNowSectionByIdQuery.cs
// Descrição: Query para buscar uma seção específica por ID (CQRS - Read)
// Autor: Will
// Empresa: WpDev
// Data: 06/12/2024
// ====================================

using MediatR;
using Portfolio.Application.DTOs.NowSections;

namespace Portfolio.Application.Queries.NowSections.GetNowSectionById;

/// <summary>
/// Query para buscar uma seção específica por ID
/// Retorna NowSectionDto completo ou null se não encontrado
/// Usado no endpoint GET /api/nowsections/{id} (ADMIN)
/// </summary>
public class GetNowSectionByIdQuery : IRequest<NowSectionDto?>
{
    // ====================================
    // PARÂMETRO DA QUERY
    // ====================================
    public Guid Id { get; set; }

    // ====================================
    // CONSTRUTOR
    // ====================================
    public GetNowSectionByIdQuery(Guid id)
    {
        Id = id;
    }
}