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
    public string IconUrl { get; set; } = string.Empty;
    public string LinkUrl { get; set; } = string.Empty;
    public string LinkText { get; set; } = string.Empty;
    public int Order { get; set; }
    public bool IsVisible { get; set; } = true;
}