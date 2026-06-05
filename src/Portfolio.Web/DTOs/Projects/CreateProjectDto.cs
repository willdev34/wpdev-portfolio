// ====================================
// Título: CreateProjectDto.cs
// Descrição: DTO para criação de projeto no admin
// ====================================

namespace Portfolio.Web.DTOs.Projects;

public class CreateProjectDto
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? ShortDescription { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public string? DemoUrl { get; set; }
    public string? RepositoryUrl { get; set; }
    public List<string> Technologies { get; set; } = new();
    public int Year { get; set; } = DateTime.Now.Year;
    public bool IsFeatured { get; set; }
    public int Status { get; set; } = 2; // 0=Planning, 1=InProgress, 2=Completed, 3=Archived
}