namespace Portfolio.Domain.Entities;

public class TimelineEvent
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public TimelineEventType Type { get; set; }
    public string? IconUrl { get; set; }
    public string? LinkUrl { get; set; }
    public string? LinkText { get; set; }
    public int Order { get; set; }
    public bool IsVisible { get; set; } = true;
    
    // Auditoria
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public enum TimelineEventType
{
    Education = 0,
    Work = 1,
    Project = 2,
    Achievement = 3,
    Certification = 4,
    Other = 5
}