// ====================================
// Título: BlogPostDto.cs
// Descrição: DTO completo de post do blog
// Autor: Will
// Empresa: WpDev
// Data: 09/12/2024
// ====================================

namespace Portfolio.Web.DTOs.BlogPosts;

public class BlogPostDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Excerpt { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string ThumbnailUrl { get; set; } = string.Empty;
    public DateTime PublishedAt { get; set; }
    public int ReadTimeMinutes { get; set; }
    public List<string> Tags { get; set; } = new();
    public bool IsPublished { get; set; }
    public int ViewCount { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}