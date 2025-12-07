// ====================================
// Título: GetActiveNowSectionQuery.cs
// Descrição: Query para buscar a seção ATIVA (CQRS - Read)
// Autor: Will
// Empresa: WpDev
// Data: 06/12/2024
// ====================================

using MediatR;
using Portfolio.Application.DTOs.NowSections;

namespace Portfolio.Application.Queries.NowSections.GetActiveNowSection;

/// <summary>
/// Query para buscar a seção Now ATIVA (IsActive = true)
/// Retorna NowSectionDto ou null se não houver seção ativa
/// Usado no endpoint GET /api/nowsection (PÚBLICO - site)
/// IMPORTANTE: Apenas 1 seção pode estar ativa por vez
/// </summary>
public class GetActiveNowSectionQuery : IRequest<NowSectionDto?>
{
    // Esta query não precisa de parâmetros
    // Ela simplesmente retorna a seção ativa
}