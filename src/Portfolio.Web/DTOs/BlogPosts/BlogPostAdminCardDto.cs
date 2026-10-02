// ====================================
// Título: BlogPostAdminCardDto.cs
// Descrição: DTO para card de post do blog na listagem do ADMIN (inclui Status e ScheduledAt)
// ====================================
namespace Portfolio.Web.DTOs.BlogPosts;
public class BlogPostAdminCardDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Excerpt { get; set; } = string.Empty;
    public string? FeaturedImageUrl { get; set; }
    public DateTime? PublishedAt { get; set; }
    public int ReadTimeMinutes { get; set; }
    public List<string> Tags { get; set; } = new();
    public string Status { get; set; } = string.Empty;
    public DateTime? ScheduledAt { get; set; }
}
