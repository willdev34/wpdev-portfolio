// ====================================
// Título: ContactMessageDto.cs
// Descrição: DTO completo do ContactMessage - usado para exibir detalhes no admin
// Autor: Will
// Empresa: WpDev
// Data: 06/12/2024
// ====================================

namespace Portfolio.Application.DTOs.ContactMessages;

/// <summary>
/// DTO completo do ContactMessage - usado para exibir detalhes completos de uma mensagem
/// Contém TODOS os dados da mensagem, incluindo IP e User Agent
/// Usado no endpoint GET /api/contactmessages/{id} (ADMIN)
/// </summary>
public class ContactMessageDto
{
    // ==========================================
    // IDENTIFICAÇÃO
    // ==========================================
    public Guid Id { get; set; }
    
    // ==========================================
    // DADOS DO REMETENTE
    // ==========================================
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    
    // ==========================================
    // TIPO E STATUS
    // ==========================================
    // Type: "General", "JobOpportunity", "Freelance", "Partnership", "Other"
    public string Type { get; set; } = string.Empty;
    
    // Status: "New", "Read", "Responded", "Archived", "Spam"
    public string Status { get; set; } = string.Empty;
    
    // ==========================================
    // SEGURANÇA E AUDITORIA
    // ==========================================
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
    
    // ==========================================
    // RESPOSTA (se houver)
    // ==========================================
    public string? ResponseMessage { get; set; }
    public DateTime? RespondedAt { get; set; }
    
    // ==========================================
    // AUDITORIA
    // ==========================================
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}