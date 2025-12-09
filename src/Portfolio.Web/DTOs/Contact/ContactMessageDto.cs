// ====================================
// Título: ContactMessageDto.cs
// Descrição: DTO para mensagens de contato
// Autor: Will
// Empresa: WpDev
// Data: 09/12/2024
// ====================================

namespace Portfolio.Web.DTOs.Contact;

public class ContactMessageDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public DateTime SentAt { get; set; }
    public bool IsRead { get; set; }
}