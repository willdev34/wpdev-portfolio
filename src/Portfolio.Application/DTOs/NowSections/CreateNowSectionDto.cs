// ====================================
// Título: CreateNowSectionDto.cs
// Descrição: DTO para criação de nova seção Now
// Autor: Will
// Empresa: WpDev
// Data: 06/12/2024
// ====================================

namespace Portfolio.Application.DTOs.NowSections;

/// <summary>
/// DTO para criação de uma nova NowSection
/// NÃO contém Id (será gerado), CreatedAt (será setado automaticamente)
/// Usado no endpoint POST /api/nowsection
/// </summary>
public class CreateNowSectionDto
{
    // ====================================
    // CONTEÚDO PRINCIPAL (obrigatório)
    // ====================================
    public string Content { get; set; } = string.Empty;
    
    // ====================================
    // PROJETO ATUAL (opcional)
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
}