// ====================================
// Título: CreateTimelineEventDto.cs
// Descrição: DTO para criação de novos eventos da timeline
// Autor: Will
// Empresa: WpDev
// Data: 29/11/2024
// ====================================

namespace Portfolio.Application.DTOs.TimelineEvents;

/// <summary>
/// DTO para criação de um novo TimelineEvent
/// NÃO contém Id (será gerado), CreatedAt (será setado automaticamente)
/// Usado no endpoint POST /api/timelineevents
/// </summary>
public class CreateTimelineEventDto
{
    // ====================================
    // INFORMAÇÕES PRINCIPAIS (obrigatórias)
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
    // MÍDIA E LINKS (opcionais)
    // ====================================
    public string? IconUrl { get; set; }
    public string? LinkUrl { get; set; }
    public string? LinkText { get; set; }
    
    // ====================================
    // ORDENAÇÃO E VISIBILIDADE
    // ====================================
    public int Order { get; set; }
    public bool IsVisible { get; set; } = true;
}