// ====================================
// Título: DeleteGalleryImageCommand.cs
// Descrição: Command para deletar uma imagem (soft delete - CQRS - Write)
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