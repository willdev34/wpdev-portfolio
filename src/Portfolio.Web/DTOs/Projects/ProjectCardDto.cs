// ====================================
// Título: ProjectCardDto.cs
// Descrição: DTO para card de projeto (listagem)
// Autor: Will
// Empresa: WpDev
// Data: 07/12/2024
// ====================================

namespace Portfolio.Web.DTOs.Projects;

public class ProjectCardDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? ShortDescription { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public List<string> Technologies { get; set; } = new();
    public int Year { get; set; }
    public bool IsFeatured { get; set; }
    public string? DemoUrl { get; set; }
    public string? RepositoryUrl { get; set; }
}