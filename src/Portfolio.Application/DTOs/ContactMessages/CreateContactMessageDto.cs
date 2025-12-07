// ====================================
// Título: CreateContactMessageDto.cs
// Descrição: DTO para criação de mensagem de contato (formulário público)
// Autor: Will
// Empresa: WpDev
// Data: 06/12/2024
// ====================================

namespace Portfolio.Application.DTOs.ContactMessages;

/// <summary>
/// DTO para criação de uma nova ContactMessage
/// Usado pelo FORMULÁRIO PÚBLICO de contato
/// NÃO contém Id, Status, IpAddress (serão setados automaticamente)
/// Usado no endpoint POST /api/contactmessages
/// </summary>
public class CreateContactMessageDto
{
    // ====================================
    // DADOS DO REMETENTE (obrigatórios)
    // ====================================
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    
    // ====================================
    // TIPO DA MENSAGEM
    // ====================================
    // 0=General, 1=JobOpportunity, 2=Freelance, 3=Partnership, 4=Other
    public int Type { get; set; }
}