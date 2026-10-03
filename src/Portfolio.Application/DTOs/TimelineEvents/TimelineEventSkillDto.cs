// ====================================
// Título: TimelineEventSkillDto.cs
// Descrição: DTOs de habilidade de um evento da timeline (leitura e entrada)
// ====================================

namespace Portfolio.Application.DTOs.TimelineEvents;

/// <summary>
/// Habilidade devolvida pela API. Category é o nome do enum (HardSkill, SoftSkill ou Stack)
/// </summary>
public class TimelineEventSkillDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public int Order { get; set; }
}

/// <summary>
/// Habilidade enviada na criação ou atualização do evento.
/// Category é o nome do enum (HardSkill, SoftSkill ou Stack). A posição na lista define a ordem.
/// </summary>
public class TimelineEventSkillInputDto
{
    public string Name { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
}
