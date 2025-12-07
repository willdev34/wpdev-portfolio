// ====================================
// Título: GalleryImageDto.cs
// Descrição: DTO completo do GalleryImage - usado para exibir detalhes de uma imagem
// Autor: Will
// Empresa: WpDev
// Data: 06/12/2024
// ====================================

namespace Portfolio.Application.DTOs.GalleryImages;

/// <summary>
/// DTO completo do GalleryImage - usado para exibir detalhes completos de uma imagem
/// Contém TODOS os dados da imagem
/// Usado no endpoint GET /api/galleryimages/{id}
/// </summary>
public class GalleryImageDto
{
    // ==========================================
    // IDENTIFICAÇÃO
    // ==========================================
    public Guid Id { get; set; }
    
    // ==========================================
    // INFORMAÇÕES PRINCIPAIS
    // ==========================================
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string AltText { get; set; } = string.Empty;
    
    // ==========================================
    // URLS DAS IMAGENS
    // ==========================================
    public string ImageUrl { get; set; } = string.Empty;
    public string? ThumbnailUrl { get; set; }
    
    // ==========================================
    // TAGS E FILTROS
    // ==========================================
    public List<string> Tags { get; set; } = new();
    
    // ==========================================
    // METADADOS DA IMAGEM
    // ==========================================
    public int Width { get; set; }
    public int Height { get; set; }
    public long FileSizeBytes { get; set; }
    
    // ==========================================
    // ORDENAÇÃO E VISIBILIDADE
    // ==========================================
    public int Order { get; set; }
    public bool IsVisible { get; set; }
    
    // ==========================================
    // AUDITORIA
    // ==========================================
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}