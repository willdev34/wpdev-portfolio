namespace Portfolio.Web.DTOs.Timeline;

public class UpdateTimelineEventDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public DateTime? EndDate { get; set; }
    public bool IsCurrent { get; set; }
    public int Type { get; set; }
    public string? IconUrl { get; set; }
    public string? LinkUrl { get; set; }
    public string? LinkText { get; set; }
    public int Order { get; set; }
    public bool IsVisible { get; set; } = true;

    // Até 5 habilidades. Nulo (omitido no JSON) mantém as atuais; lista vazia limpa;
    // lista com itens substitui por completo (a posição na lista define a ordem)
    public List<TimelineEventSkillInputDto>? Skills { get; set; }
}