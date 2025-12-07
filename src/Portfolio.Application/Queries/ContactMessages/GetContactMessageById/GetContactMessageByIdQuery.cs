// ====================================
// Título: GetContactMessageByIdQuery.cs
// Descrição: Query para buscar uma mensagem específica por ID (CQRS - Read)
// Autor: Will
// Empresa: WpDev
// Data: 06/12/2024
// ====================================

using MediatR;
using Portfolio.Application.DTOs.ContactMessages;

namespace Portfolio.Application.Queries.ContactMessages.GetContactMessageById;

/// <summary>
/// Query para buscar uma mensagem específica por ID
/// Retorna ContactMessageDto completo ou null se não encontrado
/// Usado no endpoint GET /api/contactmessages/{id} (ADMIN)
/// </summary>
public class GetContactMessageByIdQuery : IRequest<ContactMessageDto?>
{
    // ====================================
    // PARÂMETRO DA QUERY
    // ====================================
    public Guid Id { get; set; }

    // ====================================
    // CONSTRUTOR
    // ====================================
    public GetContactMessageByIdQuery(Guid id)
    {
        Id = id;
    }
}