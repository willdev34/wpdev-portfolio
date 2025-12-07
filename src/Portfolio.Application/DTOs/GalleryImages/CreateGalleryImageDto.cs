// ====================================
// Título: CreateGalleryImageDto.cs
// Descrição: DTO para criação de novas imagens na galeria
// Autor: Will
// Empresa: WpDev
// Data: 06/12/2024
// ====================================

namespace Portfolio.Application.DTOs.GalleryImages;

/// <summary>
/// DTO para criação de uma nova GalleryImage
/// NÃO contém Id (será gerado), CreatedAt (será setado automaticamente)
/// Usado no endpoint POST /api/galleryimages
/// </summary>
public class CreateGalleryImageDto
{
    // ====================================
    // INFORMAÇÕES PRINCIPAIS (obrigatórias)
    // ====================================
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string AltText { get; set; } = string.Empty;
    
    // ====================================
    // URLS DAS IMAGENS
    // ====================================
    public string ImageUrl { get; set; } = string.Empty;
    public string? ThumbnailUrl { get; set; }
    
    // ====================================
    // TAGS
    // ====================================
    public List<string> Tags { get; set; } = new();
    
    // ====================================
    // METADADOS DA IMAGEM
    // ====================================
    public int Width { get; set; }
    public int Height { get; set; }
    public long FileSizeBytes { get; set; }
    
    // ====================================
    // ORDENAÇÃO E VISIBILIDADE
    // ====================================
    public int Order { get; set; }
    public bool IsVisible { get; set; } = true;
}