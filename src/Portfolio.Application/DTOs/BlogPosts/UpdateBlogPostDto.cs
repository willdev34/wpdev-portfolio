// ====================================
// Título: UpdateBlogPostDto.cs
// Descrição: DTO para atualização de posts existentes
// ====================================

using Portfolio.Domain.Enums;

namespace Portfolio.Application.DTOs.BlogPosts;

/// <summary>
/// DTO para atualização de um BlogPost existente
/// Contém Id (para identificar qual post atualizar)
/// Usado no endpoint PUT /api/blogposts/{id}
/// </summary>
public class UpdateBlogPostDto
{
    // ====================================
    // IDENTIFICAÇÃO (obrigatório para update)
    // ====================================
    public Guid Id { get; set; }

    // ====================================
    // INFORMAÇÕES PRINCIPAIS
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