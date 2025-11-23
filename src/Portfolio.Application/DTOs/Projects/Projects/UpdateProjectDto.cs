// ====================================
// Título: UpdateProjectDto.cs (REFATORADO)
// Descrição: DTO para atualização de projetos existentes
// Autor: Will
// Empresa: WpDev
// Data: 23/11/2024
// ====================================

namespace Portfolio.Application.DTOs.Projects;

/// <summary>
/// DTO para atualização de um Project existente
/// Contém Id (para identificar qual projeto atualizar)
/// Usado no endpoint PUT /api/projects/{id}
/// </summary>
public class UpdateProjectDto
{
    // ====================================
    // IDENTIFICAÇÃO (obrigatório para update)
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
    public int Status { get; set; }
    
    // ====================================
    // CONTROLE DE ATIVAÇÃO
    // ====================================
    public bool IsActive { get; set; }
}