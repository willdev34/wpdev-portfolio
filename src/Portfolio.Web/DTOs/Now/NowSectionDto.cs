// ====================================
// Título: NowSectionDto.cs
// Descrição: DTO alinhado com a tabela NowSections do banco
// ====================================

namespace Portfolio.Web.DTOs.Now;

public class NowSectionDto
{
    public Guid Id { get; set; }
    public string Content { get; set; } = string.Empty;
    public List<ProjectLink> CurrentProjects { get; set; } = new();
    public List<string> CurrentlyLearning { get; set; } = new();
    public List<string> CurrentGoals { get; set; } = new();
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}