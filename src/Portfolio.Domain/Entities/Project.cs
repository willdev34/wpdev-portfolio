// ====================================
// Título: Project.cs (REFATORADO)
// Descrição: Entidade de domínio que representa um projeto do portfólio
// ====================================

namespace Portfolio.Domain.Entities;

/// <summary>
/// Entidade Project - Representa um projeto do portfólio
/// Design editorial minimalista (sem atributos de carta de jogo)
/// </summary>
public class Project
{
    // ====================================
    // IDENTIFICAÇÃO
    // ====================================
    public Guid Id { get; set; }
    
    // ====================================
    // INFORMAÇÕES PRINCIPAIS
    // ====================================
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? ShortDescription { get; set; }
    
    // ====================================
    // MÍDIA E LINKS
    // ====================================
    public string ImageUrl { get; set; } = string.Empty;
    public string? DemoUrl { get; set; }
    public string? RepositoryUrl { get; set; }
    
    // ====================================
    // TECNOLOGIAS E METADADOS
    // ====================================
    public List<string> Technologies { get; set; } = new();
    public int Year { get; set; }
    public bool IsFeatured { get; set; }
    public ProjectStatus Status { get; set; }
    
    // ====================================
    // AUDITORIA
    // ====================================
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public bool IsActive { get; set; } = true;
}

/// <summary>
/// Status do projeto no ciclo de desenvolvimento
/// </summary>
public enum ProjectStatus
{
    Planning = 0,      // Planejamento
    InProgress = 1,    // Em desenvolvimento
    Completed = 2,     // Concluído
    Archived = 3       // Arquivado
}