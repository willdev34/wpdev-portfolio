// ====================================
// Título: NowSectionDto.cs
// Descrição: DTO completo do NowSection - usado para exibir a seção "O que estou fazendo agora"
// Autor: Will
// Empresa: WpDev
// Data: 06/12/2024
// ====================================

namespace Portfolio.Application.DTOs.NowSections;

/// <summary>
/// DTO completo do NowSection - usado para exibir a seção completa
/// Contém TODOS os dados da seção Now
/// Usado no endpoint GET /api/nowsection
/// </summary>
public class NowSectionDto
{
    // ==========================================
    // IDENTIFICAÇÃO
    // ==========================================
    public Guid Id { get; set; }
    
    // ==========================================
    // CONTEÚDO PRINCIPAL
    // ==========================================
    public string Content { get; set; } = string.Empty;
    
    // ==========================================
    // PROJETO ATUAL
    // ==========================================
    public string? CurrentProject { get; set; }
    public string? CurrentProjectUrl { get; set; }
    
    // ==========================================
    // APRENDIZADO ATUAL
    // ==========================================
    public List<string> CurrentlyLearning { get; set; } = new();
    
    // ==========================================
    // OBJETIVOS ATUAIS
    // ==========================================
    public List<string> CurrentGoals { get; set; } = new();
    
    // ==========================================
    // STATUS
    // ==========================================
    public bool IsActive { get; set; }
    
    // ==========================================
    // AUDITORIA
    // ==========================================
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}