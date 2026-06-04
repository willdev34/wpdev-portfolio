// ====================================
// Título: DeleteNowSectionCommand.cs
// Descrição: Command para deletar uma seção (soft delete - CQRS - Write)
// ====================================

using MediatR;

namespace Portfolio.Application.Commands.NowSections.DeleteNowSection;

public class DeleteNowSectionCommand : IRequest<Unit>
{
    public Guid Id { get; set; }

    public DeleteNowSectionCommand(Guid id)
    {
        Id = id;
    }
}