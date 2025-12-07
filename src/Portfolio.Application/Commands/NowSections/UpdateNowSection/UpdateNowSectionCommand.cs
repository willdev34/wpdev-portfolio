// ====================================
// Título: UpdateNowSectionCommand.cs
// Descrição: Command para atualizar uma seção existente (CQRS - Write)
// Autor: Will
// Empresa: WpDev
// Data: 06/12/2024
// ====================================

using MediatR;
using Portfolio.Application.DTOs.NowSections;

namespace Portfolio.Application.Commands.NowSections.UpdateNowSection;

/// <summary>
/// Command para atualizar uma seção existente
/// Recebe os dados do UpdateNowSectionDto
/// Retorna Unit (void) - não retorna nada em caso de sucesso
/// IMPORTANTE: Se IsActive mudar para true, desativa todas as outras
/// Usado no endpoint PUT /api/nowsections/{id}
/// </summary>
public class UpdateNowSectionCommand : IRequest<Unit>
{
    public UpdateNowSectionDto SectionData { get; set; } = null!;

    public UpdateNowSectionCommand(UpdateNowSectionDto sectionData)
    {
        SectionData = sectionData;
    }
}