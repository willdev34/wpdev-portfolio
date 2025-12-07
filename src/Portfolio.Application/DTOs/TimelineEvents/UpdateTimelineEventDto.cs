// ====================================
// Título: UpdateTimelineEventDto.cs
// Descrição: DTO para atualização de eventos existentes
// Autor: Will
// Empresa: WpDev
// Data: 29/11/2024
// ====================================

namespace Portfolio.Application.DTOs.TimelineEvents;

/// <summary>
/// DTO para atualização de um TimelineEvent existente
/// Contém Id (para identificar qual evento atualizar)
/// Usado no endpoint PUT /api/timelineevents/{id}
/// </summary>
public class UpdateTimelineEventDto
{
    // ====================================
    // IDENTIFICAÇÃO (obrigatório para update)
    // ====================================
    public Guid Id { get; set; }
    
    // ====================================
    // INFORMAÇÕES PRINCIPAIS
    // ====================================
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    
    // ====================================
    // TIPO DO EVENTO
    // ====================================
    // 0=Education, 1=Work, 2=Project, 3=Achievement, 4=Certification, 5=Other
    public int Type { get; set; }
    
    // ====================================
    // MÍDIA E LINKS
    // ====================================
    public string? IconUrl { get; set; }
    public string? LinkUrl { get; set; }
    public string? LinkText { get; set; }
    
    // ====================================
    // ORDENAÇÃO E VISIBILIDADE
    // ====================================
    public int Order { get; set; }
    public bool IsVisible { get; set; }
}