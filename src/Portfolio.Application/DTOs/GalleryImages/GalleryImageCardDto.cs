// ====================================
// Título: GalleryImageCardDto.cs
// Descrição: DTO resumido do GalleryImage - usado para grid de galeria
// Autor: Will
// Empresa: WpDev
// Data: 06/12/2024
// ====================================

namespace Portfolio.Application.DTOs.GalleryImages;

/// <summary>
/// DTO simplificado do GalleryImage - usado para exibição em grid de galeria
/// Contém apenas os dados essenciais para exibir a imagem no grid
/// Reduz tráfego de rede ao listar muitas imagens
/// Usado no endpoint GET /api/galleryimages
/// </summary>
public class GalleryImageCardDto
{
    // ==========================================
    // IDENTIFICAÇÃO
    // ==========================================
    public Guid Id { get; set; }
    
    // ==========================================
    // INFORMAÇÕES PRINCIPAIS (resumidas)
    // ==========================================
    public string Title { get; set; } = string.Empty;
    public string AltText { get; set; } = string.Empty;
    
    // ==========================================
    // URLS DAS IMAGENS
    // ==========================================
    public string? ThumbnailUrl { get; set; }
    
    // ==========================================
    // TAGS
    // ==========================================
    public List<string> Tags { get; set; } = new();
    
    // ==========================================
    // ORDENAÇÃO
    // ==========================================
    public int Order { get; set; }
}