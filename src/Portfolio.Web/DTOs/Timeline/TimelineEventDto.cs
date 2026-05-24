// ====================================
// Título: TimelineEventDto.cs
// Descrição: DTO para eventos da timeline profissional
// ====================================

namespace Portfolio.Web.DTOs.Timeline;

public class TimelineEventDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public string Type { get; set; } = string.Empty;
    public string IconUrl { get; set; } = string.Empty;
    public string LinkUrl { get; set; } = string.Empty;
    public string LinkText { get; set; } = string.Empty;
    public int Order { get; set; }
}