// ====================================
// Título: CreateNowSectionCommand.cs
// Descrição: Command para criar uma nova seção Now (CQRS - Write)
// Autor: Will
// Empresa: WpDev
// Data: 06/12/2024
// ====================================

using MediatR;
using Portfolio.Application.DTOs.NowSections;

namespace Portfolio.Application.Commands.NowSections.CreateNowSection;

/// <summary>
/// Command para criar uma nova seção Now
/// Recebe os dados do CreateNowSectionDto e retorna o NowSectionDto completo
/// IMPORTANTE: Desativa todas as seções anteriores antes de criar (business rule)
/// Usado no endpoint POST /api/nowsections
/// </summary>
public class CreateNowSectionCommand : IRequest<NowSectionDto>
{
    // ====================================
    // DADOS DA SEÇÃO A SER CRIADA
    // ====================================
    public CreateNowSectionDto SectionData { get; set; } = null!;

    // ====================================
    // CONSTRUTOR
    // ====================================
    public CreateNowSectionCommand(CreateNowSectionDto sectionData)
    {
        SectionData = sectionData;
    }
}