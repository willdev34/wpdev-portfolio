// ====================================
// Título: UpdateContactMessageDto.cs
// Descrição: DTO para atualização de mensagem (ADMIN apenas)
// Autor: Will
// Empresa: WpDev
// Data: 06/12/2024
// ====================================

namespace Portfolio.Application.DTOs.ContactMessages;

/// <summary>
/// DTO para atualização de uma ContactMessage existente
/// Usado pelo ADMIN para mudar status, adicionar resposta, marcar como spam
/// Usado no endpoint PUT /api/contactmessages/{id} (ADMIN)
/// </summary>
public class UpdateContactMessageDto
{
    // ====================================
    // IDENTIFICAÇÃO (obrigatório para update)
    // ====================================
    public Guid Id { get; set; }
    
    // ====================================
    // STATUS
    // ====================================
    // 0=New, 1=Read, 2=Responded, 3=Archived, 4=Spam
    public int Status { get; set; }
    
    // ====================================
    // RESPOSTA DO ADMIN (opcional)
    // ====================================
    public string? ResponseMessage { get; set; }
}