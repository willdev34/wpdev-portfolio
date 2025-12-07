// ====================================
// Título: TimelineEventDto.cs
// Descrição: DTO completo do TimelineEvent - usado para exibir detalhes de um evento
// Autor: Will
// Empresa: WpDev
// Data: 29/11/2024
// ====================================

namespace Portfolio.Application.DTOs.TimelineEvents;

/// <summary>
/// DTO completo do TimelineEvent - usado para exibir detalhes completos de um evento da timeline
/// Contém TODOS os dados do evento
/// Usado no endpoint GET /api/timelineevents/{id}
/// </summary>
public class TimelineEventDto
{
    // ==========================================
    // IDENTIFICAÇÃO
    // ==========================================
    public Guid Id { get; set; }
    
    // ==========================================
    // INFORMAÇÕES PRINCIPAIS
    // ==========================================
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    
    // ==========================================
    // TIPO DO EVENTO
    // ==========================================
    // "Education", "Work", "Project", "Achievement", "Certification", "Other"
    public string Type { get; set; } = string.Empty;
    
    // ==========================================
    // MÍDIA E LINKS
    // ==========================================
    public string? IconUrl { get; set; }
    public string? LinkUrl { get; set; }
    public string? LinkText { get; set; }
    
    // ==========================================
    // ORDENAÇÃO E VISIBILIDADE
    // ==========================================
    public int Order { get; set; }
    public bool IsVisible { get; set; }
    
    // ==========================================
    // AUDITORIA
    // ==========================================
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}