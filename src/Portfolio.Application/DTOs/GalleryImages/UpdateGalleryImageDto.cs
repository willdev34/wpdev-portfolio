// ====================================
// Título: UpdateGalleryImageDto.cs
// Descrição: DTO para atualização de imagens existentes
// Autor: Will
// Empresa: WpDev
// Data: 06/12/2024
// ====================================

namespace Portfolio.Application.DTOs.GalleryImages;

/// <summary>
/// DTO para atualização de uma GalleryImage existente
/// Contém Id (para identificar qual imagem atualizar)
/// Usado no endpoint PUT /api/galleryimages/{id}
/// </summary>
public class UpdateGalleryImageDto
{
    // ====================================
    // IDENTIFICAÇÃO (obrigatório para update)
    // ====================================
    public Guid Id { get; set; }
    
    // ====================================
    // INFORMAÇÕES PRINCIPAIS
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
    public bool IsVisible { get; set; }
}