// ====================================
// Título: BlogPostAdminCardDto.cs
// Descrição: DTO resumido do BlogPost para a listagem do ADMIN
// ====================================

namespace Portfolio.Application.DTOs.BlogPosts;

/// <summary>
/// DTO simplificado do BlogPost para a listagem do ADMIN (GET /api/blogposts)
/// Igual ao BlogPostCardDto, mas inclui Status e ScheduledAt
/// Esses dois campos NUNCA devem ir para o DTO público (BlogPostCardDto)
/// </summary>
public class BlogPostAdminCardDto
{
    // ==========================================
    // IDENTIFICAÇÃO
    // ==========================================
    public Guid Id { get; set; }

    // ==========================================
    // INFORMAÇÕES PRINCIPAIS (resumidas)
    // ==========================================
    public string Title { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string Excerpt { get; set; } = string.Empty;

    // ==========================================
    // MÍDIA
    // ==========================================
    public string? FeaturedImageUrl { get; set; }

    // ==========================================
    // METADADOS BÁSICOS
    // ==========================================
    public List<string> Tags { get; set; } = new();
    public bool IsFeatured { get; set; }
    public DateTime? PublishedAt { get; set; }
    public int ReadTimeMinutes { get; set; }

    // ==========================================
    // ESTATÍSTICAS
    // ==========================================
    public int ViewCount { get; set; }

    // ==========================================
    // STATUS (apenas no admin)
    // ==========================================
    public string Status { get; set; } = string.Empty;
    public DateTime? ScheduledAt { get; set; }
}
