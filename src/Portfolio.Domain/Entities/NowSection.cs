namespace Portfolio.Domain.Entities;

public class NowSection
{
    public Guid Id { get; set; }
    public string Content { get; set; } = string.Empty;
    public string? CurrentProject { get; set; }
    public string? CurrentProjectUrl { get; set; }
    public List<string> CurrentlyLearning { get; set; } = new();
    public List<string> CurrentGoals { get; set; } = new();
    public bool IsActive { get; set; } = true;
    
    // Auditoria
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}