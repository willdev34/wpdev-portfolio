// ====================================
// Título: CreateGalleryImageCommand.cs
// Descrição: Command para criar uma nova imagem na galeria (CQRS - Write)
// ====================================

using MediatR;
using Portfolio.Application.DTOs.GalleryImages;

namespace Portfolio.Application.Commands.GalleryImages.CreateGalleryImage;

/// <summary>
/// Command para criar uma nova imagem na galeria
/// Recebe os dados do CreateGalleryImageDto e retorna o GalleryImageDto completo da imagem criada
/// Usado no endpoint POST /api/galleryimages
/// </summary>
public class CreateGalleryImageCommand : IRequest<GalleryImageDto>
{
    // ====================================
    // DADOS DA IMAGEM A SER CRIADA
    // ====================================
    public CreateGalleryImageDto ImageData { get; set; } = null!;

    // ====================================
    // CONSTRUTOR
    // ====================================
    public CreateGalleryImageCommand(CreateGalleryImageDto imageData)
    {
        ImageData = imageData;
    }
}