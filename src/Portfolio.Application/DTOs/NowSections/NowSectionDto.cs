// ====================================
// Título: NowSectionDto.cs
// Descrição: DTO completo do NowSection - usado para exibir a seção "O que estou fazendo agora"
// ====================================

using Portfolio.Domain.ValueObjects;

namespace Portfolio.Application.DTOs.NowSections;

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