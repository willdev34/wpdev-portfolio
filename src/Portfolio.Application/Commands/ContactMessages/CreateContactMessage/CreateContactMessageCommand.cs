// ====================================
// Título: CreateContactMessageCommand.cs
// Descrição: Command para criar uma nova mensagem de contato (CQRS - Write)
// Autor: Will
// Empresa: WpDev
// Data: 06/12/2024
// ====================================

using MediatR;
using Portfolio.Application.DTOs.ContactMessages;

namespace Portfolio.Application.Commands.ContactMessages.CreateContactMessage;

/// <summary>
/// Command para criar uma nova mensagem de contato
/// Recebe os dados do CreateContactMessageDto e retorna o ContactMessageDto completo
/// Usado no endpoint POST /api/contactmessages (FORMULÁRIO PÚBLICO)
/// </summary>
public class CreateContactMessageCommand : IRequest<ContactMessageDto>
{
    // ====================================
    // DADOS DA MENSAGEM A SER CRIADA
    // ====================================
    public CreateContactMessageDto MessageData { get; set; } = null!;
    
    // ====================================
    // DADOS DE CONTEXTO HTTP (serão injetados)
    // ====================================
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }

    // ====================================
    // CONSTRUTOR
    // ====================================
    public CreateContactMessageCommand(CreateContactMessageDto messageData)
    {
        MessageData = messageData;
    }
}