// ====================================
// Título: CreateBlogPostDto.cs
// Descrição: DTO para criação de novos posts do blog
// ====================================

using Portfolio.Domain.Enums;

namespace Portfolio.Application.DTOs.BlogPosts;

/// <summary>
/// DTO para criação de um novo BlogPost
/// NÃO contém Id (será gerado), CreatedAt (será setado automaticamente)
/// Usado no endpoint POST /api/blogposts
/// </summary>
public class CreateBlogPostDto
{
    // ====================================
    // INFORMAÇÕES PRINCIPAIS (obrigatórias)
    // ====================================
    public string Title { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string Excerpt { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;

    // ====================================
    // MÍDIA
    // ====================================
    public string? FeaturedImageUrl { get; set; }

    // ====================================
    // METADADOS
    // ====================================
    public List<string> Tags { get; set; } = new();
    public bool IsFeatured { get; set; }
    public bool IsPublished { get; set; }
    public int ReadTimeMinutes { get; set; }

    // ====================================
    // STATUS (nova fonte de verdade)
    // ====================================
    // Se nulo, o handler deriva de IsPublished (compatibilidade com o formulário atual)
    public BlogPostStatus? Status { get; set; }
    public DateTime? ScheduledAt { get; set; }
}