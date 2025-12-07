// ====================================
// Título: ContactMessageCardDto.cs
// Descrição: DTO resumido do ContactMessage - usado para listagem no admin
// Autor: Will
// Empresa: WpDev
// Data: 06/12/2024
// ====================================

namespace Portfolio.Application.DTOs.ContactMessages;

/// <summary>
/// DTO simplificado do ContactMessage - usado para listagem no painel admin
/// Contém apenas os dados essenciais para exibir na lista
/// Reduz tráfego de rede ao listar muitas mensagens
/// Usado no endpoint GET /api/contactmessages (ADMIN)
/// </summary>
public class ContactMessageCardDto
{
    // ==========================================
    // IDENTIFICAÇÃO
    // ==========================================
    public Guid Id { get; set; }
    
    // ==========================================
    // DADOS BÁSICOS
    // ==========================================
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    
    // ==========================================
    // TIPO E STATUS
    // ==========================================
    public string Type { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    
    // ==========================================
    // DATA
    // ==========================================
    public DateTime CreatedAt { get; set; }
}