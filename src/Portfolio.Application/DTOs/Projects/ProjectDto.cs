namespace Portfolio.Application.DTOs.Projects;

/// <summary>
/// DTO completo do Project - usado para exibir detalhes completos de um projeto
/// </summary>
public class ProjectDto
{
    // ==========================================
    // IDENTIFICAÇÃO
    // ==========================================
    public Guid Id { get; set; }
    
    // ==========================================
    // INFORMAÇÕES PRINCIPAIS
    // ==========================================
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? ShortDescription { get; set; }
    
    // ==========================================
    // MÍDIA E LINKS
    // ==========================================
    public string ImageUrl { get; set; } = string.Empty;
    public string? DemoUrl { get; set; }
    public string? RepositoryUrl { get; set; }
    
    // ==========================================
    // TECNOLOGIAS E METADADOS
    // ==========================================
    public List<string> Technologies { get; set; } = new();
    public int Year { get; set; }
    public bool IsFeatured { get; set; }
    public string Status { get; set; } = string.Empty; // "Planning", "InProgress", "Completed", "Archived"
    
    // ==========================================
    // AUDITORIA
    // ==========================================
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public bool IsActive { get; set; }
}