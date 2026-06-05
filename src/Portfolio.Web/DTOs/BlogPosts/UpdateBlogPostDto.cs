// ====================================
// Título: UpdateBlogPostDto.cs
// Descrição: DTO para atualização de post do blog no admin
// ====================================

namespace Portfolio.Web.DTOs.BlogPosts;

public class UpdateBlogPostDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string Excerpt { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string? FeaturedImageUrl { get; set; }
    public List<string> Tags { get; set; } = new();
    public bool IsFeatured { get; set; }
    public bool IsPublished { get; set; }
    public int ReadTimeMinutes { get; set; }
}