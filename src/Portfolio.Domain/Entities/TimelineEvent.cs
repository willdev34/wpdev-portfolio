namespace Portfolio.Domain.Entities;

public class TimelineEvent
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public DateTime? EndDate { get; set; }
    public bool IsCurrent { get; set; } = false;
    public TimelineEventType Type { get; set; }
    public string? IconUrl { get; set; }
    public string? LinkUrl { get; set; }
    public string? LinkText { get; set; }
    // Em desuso: a ordem da timeline agora é automática (data inicial, data final, CreatedAt).
    // A coluna e o índice continuam no banco; nenhuma consulta usa este campo.
    public int Order { get; set; }
    public bool IsVisible { get; set; } = true;

    // Habilidades do evento (até 5), na ordem de TimelineEventSkill.Order
    public ICollection<TimelineEventSkill> Skills { get; set; } = new List<TimelineEventSkill>();

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