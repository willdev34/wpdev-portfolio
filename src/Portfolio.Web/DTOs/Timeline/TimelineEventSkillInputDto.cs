namespace Portfolio.Web.DTOs.Timeline;

// Habilidade enviada na criação ou atualização do evento
public class TimelineEventSkillInputDto
{
    public string Name { get; set; } = string.Empty;
    // HardSkill, SoftSkill ou Stack
    public string Category { get; set; } = "HardSkill";
}
