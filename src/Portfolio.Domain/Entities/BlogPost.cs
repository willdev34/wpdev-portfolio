namespace Portfolio.Domain.Entities;

public class BlogPost
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
    public DateTime? PublishedAt { get; set; }
    public int ReadTimeMinutes { get; set; }
    public int ViewCount { get; set; }
    
    // Auditoria
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    
    // Relacionamento (futuramente com User)
    public Guid? AuthorId { get; set; }
}