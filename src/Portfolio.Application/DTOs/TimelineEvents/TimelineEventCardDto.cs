// ====================================
// Título: TimelineEventCardDto.cs
// Descrição: DTO resumido do TimelineEvent - usado para listagens
// Autor: Will
// Empresa: WpDev
// Data: 29/11/2024
// ====================================

namespace Portfolio.Application.DTOs.TimelineEvents;

/// <summary>
/// DTO simplificado do TimelineEvent - usado para listagens e exibição na timeline
/// Contém apenas os dados essenciais para exibir o card do evento
/// Reduz tráfego de rede ao listar muitos eventos
/// Usado no endpoint GET /api/timelineevents
/// </summary>
public class TimelineEventCardDto
{
    // ==========================================
    // IDENTIFICAÇÃO
    // ==========================================
    public Guid Id { get; set; }
    
    // ==========================================
    // INFORMAÇÕES PRINCIPAIS (resumidas)
    // ==========================================
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    
    // ==========================================
    // TIPO DO EVENTO
    // ==========================================
    public string Type { get; set; } = string.Empty;
    
    // ==========================================
    // MÍDIA
    // ==========================================
    public string? IconUrl { get; set; }
    
    // ==========================================
    // ORDENAÇÃO
    // ==========================================
    public int Order { get; set; }
}