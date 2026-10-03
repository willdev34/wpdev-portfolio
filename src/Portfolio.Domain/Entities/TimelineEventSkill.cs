using Portfolio.Domain.Enums;

namespace Portfolio.Domain.Entities;

/// <summary>
/// Habilidade desenvolvida ou usada em um evento da timeline (máximo de 5 por evento)
/// Excluída em cascata junto com o evento (remoção física)
/// </summary>
public class TimelineEventSkill
{
    public Guid Id { get; set; }
    public Guid TimelineEventId { get; set; }
    public string Name { get; set; } = string.Empty;
    public SkillCategory Category { get; set; }

    // Posição na lista informada pelo admin (0, 1, 2...)
    public int Order { get; set; }

    public TimelineEvent? TimelineEvent { get; set; }
}
