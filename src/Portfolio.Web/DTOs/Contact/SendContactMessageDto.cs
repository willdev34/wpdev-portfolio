// ====================================
// Título: SendContactMessageDto.cs
// Descrição: DTO para envio do formulário de contato
//            Alinhado com CreateContactMessageDto da API
// ====================================

namespace Portfolio.Web.DTOs.Contact;

public class SendContactMessageDto
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;

    // 0=General, 1=JobOpportunity, 2=Freelance, 3=Partnership, 4=Other
    public int Type { get; set; } = 0;
}