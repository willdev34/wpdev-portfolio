// ====================================
// Título: UpdateGalleryImageCommand.cs
// Descrição: Command para atualizar uma imagem existente (CQRS - Write)
// Autor: Will
// Empresa: WpDev
// Data: 06/12/2024
// ====================================

using MediatR;
using Portfolio.Application.DTOs.GalleryImages;

namespace Portfolio.Application.Commands.GalleryImages.UpdateGalleryImage;

/// <summary>
/// Command para atualizar uma imagem existente
/// Recebe os dados do UpdateGalleryImageDto
/// Retorna Unit (void) - não retorna nada em caso de sucesso
/// Usado no endpoint PUT /api/galleryimages/{id}
/// </summary>
public class UpdateGalleryImageCommand : IRequest<Unit>
{
    public UpdateGalleryImageDto ImageData { get; set; } = null!;

    public UpdateGalleryImageCommand(UpdateGalleryImageDto imageData)
    {
        ImageData = imageData;
    }
}