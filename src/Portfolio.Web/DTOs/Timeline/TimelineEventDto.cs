namespace Portfolio.Web.DTOs.Timeline;

public class TimelineEventDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public DateTime? EndDate { get; set; }
    public bool IsCurrent { get; set; }
    public string Type { get; set; } = string.Empty;
    public string? IconUrl { get; set; }
    public string? LinkUrl { get; set; }
    public string? LinkText { get; set; }
    public int Order { get; set; }
    public bool IsVisible { get; set; }
    public List<TimelineEventSkillDto> Skills { get; set; } = new();
}

// Mesmo arquivo do TimelineEventDto de propósito: o teste de regressão linka só este arquivo
public class TimelineEventSkillDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    // HardSkill, SoftSkill ou Stack
    public string Category { get; set; } = string.Empty;
    public int Order { get; set; }
}