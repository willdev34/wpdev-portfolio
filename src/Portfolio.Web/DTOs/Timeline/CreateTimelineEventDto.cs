namespace Portfolio.Web.DTOs.Timeline;

public class CreateTimelineEventDto
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime Date { get; set; } = DateTime.Today;
    public DateTime? EndDate { get; set; }
    public bool IsCurrent { get; set; }
    public int Type { get; set; } = 1;
    public string IconUrl { get; set; } = string.Empty;
    public string LinkUrl { get; set; } = string.Empty;
    public string LinkText { get; set; } = string.Empty;
    public int Order { get; set; }
    public bool IsVisible { get; set; } = true;
}