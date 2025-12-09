// ====================================
// Título: TimelineEventDto.cs
// Descrição: DTO para eventos da timeline profissional
// Autor: Will
// Empresa: WpDev
// Data: 09/12/2024
// ====================================

namespace Portfolio.Web.DTOs.Timeline;

public class TimelineEventDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Company { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string IconUrl { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public bool IsCurrent { get; set; }
    public int DisplayOrder { get; set; }
}