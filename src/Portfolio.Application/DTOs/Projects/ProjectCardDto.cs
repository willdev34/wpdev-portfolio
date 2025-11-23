namespace Portfolio.Application.DTOs.Projects;

/// <summary>
/// DTO simplificado do Project - usado para listagens e grid de cards
/// Contém apenas os dados essenciais para exibir o card estilo Yu-Gi-Oh!
/// Reduz tráfego de rede ao listar muitos projetos
/// </summary>
public class ProjectCardDto
{
    // ==========================================
    // IDENTIFICAÇÃO
    // ==========================================
    public Guid Id { get; set; }
    
    // ==========================================
    // INFORMAÇÕES PRINCIPAIS (resumidas)
    // ==========================================
    public string Title { get; set; } = string.Empty;
    public string? ShortDescription { get; set; }
    
    // ==========================================
    // MÍDIA
    // ==========================================
    public string ImageUrl { get; set; } = string.Empty;
    
    // ==========================================
    // TECNOLOGIAS (primeiras 3 tags principais)
    // ==========================================
    public List<string> Technologies { get; set; } = new();
    
    // ==========================================
    // METADADOS BÁSICOS
    // ==========================================
    public int Year { get; set; }
    public bool IsFeatured { get; set; }
    
    // ==========================================
    // ATRIBUTOS ESTILO CARTA (para o visual)
    // ==========================================
    public string Rarity { get; set; } = string.Empty;
    public int AttackPoints { get; set; }
    public int DefensePoints { get; set; }
    
    // ==========================================
    // LINKS (apenas para navegação rápida)
    // ==========================================
    public string? DemoUrl { get; set; }
    public string? RepositoryUrl { get; set; }
}