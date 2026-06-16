using Portfolio.Domain.ValueObjects;

namespace Portfolio.Domain.Entities;

public class NowSection
{
    public Guid Id { get; set; }
    public string Content { get; set; } = string.Empty;
    public List<ProjectLink> CurrentProjects { get; set; } = new();
    public List<string> CurrentlyLearning { get; set; } = new();
    public List<string> CurrentGoals { get; set; } = new();
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}