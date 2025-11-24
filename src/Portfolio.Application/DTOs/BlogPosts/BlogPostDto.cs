// ====================================
// Título: BlogPostDto.cs
// Descrição: DTO completo do BlogPost - usado para exibir detalhes de um post
// Autor: Will
// Empresa: WpDev
// Data: 23/11/2024
// ====================================

namespace Portfolio.Application.DTOs.BlogPosts;

/// <summary>
/// DTO completo do BlogPost - usado para exibir detalhes completos de um post do blog
/// Contém TODOS os dados, incluindo o conteúdo Markdown completo
/// Usado no endpoint GET /api/blogposts/{id}
/// </summary>
public class BlogPostDto
{
    // ==========================================
    // IDENTIFICAÇÃO
    // ==========================================
    public Guid Id { get; set; }
    
    // ==========================================
    // INFORMAÇÕES PRINCIPAIS
    // ==========================================
    public string Title { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string Excerpt { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    
    // ==========================================
    // MÍDIA
    // ==========================================
    public string? FeaturedImageUrl { get; set; }
    
    // ==========================================
    // METADADOS
    // ==========================================
    public List<string> Tags { get; set; } = new();
    public bool IsFeatured { get; set; }
    public bool IsPublished { get; set; }
    public DateTime? PublishedAt { get; set; }
    public int ReadTimeMinutes { get; set; }
    public int ViewCount { get; set; }
    
    // ==========================================
    // AUDITORIA
    // ==========================================
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    
    // ==========================================
    // RELACIONAMENTO (futuramente com User)
    // ==========================================
    public Guid? AuthorId { get; set; }
}