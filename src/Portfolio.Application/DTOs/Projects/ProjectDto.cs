namespace Portfolio.Application.DTOs.Projects;

/// <summary>
/// DTO completo do Project - usado para exibir detalhes completos de um projeto
/// Contém TODOS os dados, incluindo atributos estilo carta de jogo
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
    // ATRIBUTOS ESTILO CARTA YU-GI-OH!
    // ==========================================
    public string Rarity { get; set; } = string.Empty; // "Common", "Rare", "SuperRare", "UltraRare", "Secret"
    public int AttackPoints { get; set; }
    public int DefensePoints { get; set; }
    public string? FlavorText { get; set; }
    
    // ==========================================
    // AUDITORIA
    // ==========================================
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public bool IsActive { get; set; }
}