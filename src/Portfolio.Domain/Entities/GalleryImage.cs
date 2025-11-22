namespace Portfolio.Domain.Entities;

public class GalleryImage
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public string? ThumbnailUrl { get; set; }
    public string AltText { get; set; } = string.Empty;
    public List<string> Tags { get; set; } = new();
    public int Order { get; set; }
    public bool IsVisible { get; set; } = true;
    
    // Metadados da imagem
    public int Width { get; set; }
    public int Height { get; set; }
    public long FileSizeBytes { get; set; }
    
    // Auditoria
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}