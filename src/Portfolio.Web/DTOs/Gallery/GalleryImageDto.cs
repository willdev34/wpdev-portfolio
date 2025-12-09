// ====================================
// Título: GalleryImageDto.cs
// Descrição: DTO para imagens da galeria
// Autor: Will
// Empresa: WpDev
// Data: 09/12/2024
// ====================================

namespace Portfolio.Web.DTOs.Gallery;

public class GalleryImageDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public string ThumbnailUrl { get; set; } = string.Empty;
    public List<string> Tags { get; set; } = new();
    public DateTime CapturedAt { get; set; }
    public int DisplayOrder { get; set; }
}