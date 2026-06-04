// ====================================
// Título: BlogPostCardDto.cs
// Descrição: DTO para card de post do blog (listagem)
// ====================================
namespace Portfolio.Web.DTOs.BlogPosts;
public class BlogPostCardDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Excerpt { get; set; } = string.Empty;
    public string FeaturedImageUrl { get; set; } = string.Empty;
    public DateTime PublishedAt { get; set; }
    public int ReadTimeMinutes { get; set; }
    public List<string> Tags { get; set; } = new();
}