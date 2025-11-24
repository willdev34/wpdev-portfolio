// ====================================
// Título: BlogPostCardDto.cs
// Descrição: DTO resumido do BlogPost - usado para listagens e grid de cards
// Autor: Will
// Empresa: WpDev
// Data: 23/11/2024
// ====================================

namespace Portfolio.Application.DTOs.BlogPosts;

/// <summary>
/// DTO simplificado do BlogPost - usado para listagens e grid de cards
/// Contém apenas os dados essenciais para exibir o card do post (título, excerpt, imagem, tags)
/// Reduz tráfego de rede ao listar muitos posts
/// Usado no endpoint GET /api/blogposts
/// </summary>
public class BlogPostCardDto
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
}