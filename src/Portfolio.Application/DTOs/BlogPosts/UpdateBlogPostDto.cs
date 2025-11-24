// ====================================
// Título: UpdateBlogPostDto.cs
// Descrição: DTO para atualização de posts existentes
// Autor: Will
// Empresa: WpDev
// Data: 23/11/2024
// ====================================

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
}