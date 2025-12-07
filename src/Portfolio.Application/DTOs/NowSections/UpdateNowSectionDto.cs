// ====================================
// Título: UpdateNowSectionDto.cs
// Descrição: DTO para atualização de seção Now existente
// Autor: Will
// Empresa: WpDev
// Data: 06/12/2024
// ====================================

namespace Portfolio.Application.DTOs.NowSections;

/// <summary>
/// DTO para atualização de uma NowSection existente
/// Contém Id (para identificar qual seção atualizar)
/// Usado no endpoint PUT /api/nowsection/{id}
/// </summary>
public class UpdateNowSectionDto
{
    // ====================================
    // IDENTIFICAÇÃO (obrigatório para update)
    // ====================================
    public Guid Id { get; set; }
    
    // ====================================
    // CONTEÚDO PRINCIPAL
    // ====================================
    public string Content { get; set; } = string.Empty;
    
    // ====================================
    // PROJETO ATUAL
    // ====================================
    public string? CurrentProject { get; set; }
    public string? CurrentProjectUrl { get; set; }
    
    // ====================================
    // APRENDIZADO ATUAL
    // ====================================
    public List<string> CurrentlyLearning { get; set; } = new();
    
    // ====================================
    // OBJETIVOS ATUAIS
    // ====================================
    public List<string> CurrentGoals { get; set; } = new();
    
    // ====================================
    // STATUS
    // ====================================
    public bool IsActive { get; set; }
}