// ====================================
// Título: CreateProjectDto.cs (REFATORADO)
// Descrição: DTO para criação de novos projetos
// Autor: Will
// Empresa: WpDev
// Data: 23/11/2024
// ====================================

namespace Portfolio.Application.DTOs.Projects;

/// <summary>
/// DTO para criação de um novo Project
/// NÃO contém Id (será gerado), CreatedAt (será setado automaticamente)
/// Usado no endpoint POST /api/projects
/// </summary>
public class CreateProjectDto
{
    // ====================================
    // INFORMAÇÕES PRINCIPAIS (obrigatórias)
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
    public int Status { get; set; } // 0=Planning, 1=InProgress, 2=Completed, 3=Archived
}