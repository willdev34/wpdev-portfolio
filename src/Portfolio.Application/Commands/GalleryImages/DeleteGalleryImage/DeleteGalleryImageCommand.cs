// ====================================
// Título: DeleteGalleryImageCommand.cs
// Descrição: Command para deletar uma imagem (soft delete - CQRS - Write)
// Autor: Will
// Empresa: WpDev
// Data: 06/12/2024
// ====================================

using MediatR;

namespace Portfolio.Application.Commands.GalleryImages.DeleteGalleryImage;

public class DeleteGalleryImageCommand : IRequest<Unit>
{
    public Guid Id { get; set; }

    public DeleteGalleryImageCommand(Guid id)
    {
        Id = id;
    }
}