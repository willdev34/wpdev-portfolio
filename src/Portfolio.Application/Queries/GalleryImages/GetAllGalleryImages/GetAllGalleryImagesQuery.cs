// ====================================
// Título: GetAllGalleryImagesQuery.cs
// Descrição: Query para buscar todas as imagens da galeria (CQRS - Read)
// Autor: Will
// Empresa: WpDev
// Data: 06/12/2024
// ====================================

using MediatR;
using Portfolio.Application.DTOs.GalleryImages;

namespace Portfolio.Application.Queries.GalleryImages.GetAllGalleryImages;

/// <summary>
/// Query para buscar TODAS as imagens da galeria
/// Retorna uma lista de GalleryImageCardDto (versão simplificada)
/// Ordenadas por Order (crescente)
/// Usado no endpoint GET /api/galleryimages
/// </summary>
public class GetAllGalleryImagesQuery : IRequest<IEnumerable<GalleryImageCardDto>>
{
    // Esta query não precisa de parâmetros
    // Ela simplesmente retorna TODAS as imagens
}