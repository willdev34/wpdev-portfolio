// ====================================
// Título: GetGalleryImageByIdQuery.cs
// Descrição: Query para buscar uma imagem específica por ID (CQRS - Read)
// Autor: Will
// Empresa: WpDev
// Data: 06/12/2024
// ====================================

using MediatR;
using Portfolio.Application.DTOs.GalleryImages;

namespace Portfolio.Application.Queries.GalleryImages.GetGalleryImageById;

/// <summary>
/// Query para buscar uma imagem específica por ID
/// Retorna GalleryImageDto completo ou null se não encontrado
/// Usado no endpoint GET /api/galleryimages/{id}
/// </summary>
public class GetGalleryImageByIdQuery : IRequest<GalleryImageDto?>
{
    // ====================================
    // PARÂMETRO DA QUERY
    // ====================================
    public Guid Id { get; set; }

    // ====================================
    // CONSTRUTOR
    // ====================================
    public GetGalleryImageByIdQuery(Guid id)
    {
        Id = id;
    }
}