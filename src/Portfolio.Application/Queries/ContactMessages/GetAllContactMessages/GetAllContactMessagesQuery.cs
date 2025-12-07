// ====================================
// Título: GetAllContactMessagesQuery.cs
// Descrição: Query para buscar todas as mensagens de contato (CQRS - Read)
// Autor: Will
// Empresa: WpDev
// Data: 06/12/2024
// ====================================

using MediatR;
using Portfolio.Application.DTOs.ContactMessages;

namespace Portfolio.Application.Queries.ContactMessages.GetAllContactMessages;

/// <summary>
/// Query para buscar TODAS as mensagens de contato
/// Retorna uma lista de ContactMessageCardDto (versão simplificada)
/// Ordenadas por CreatedAt (mais recentes primeiro)
/// Usado no endpoint GET /api/contactmessages (ADMIN)
/// </summary>
public class GetAllContactMessagesQuery : IRequest<IEnumerable<ContactMessageCardDto>>
{
    // Esta query não precisa de parâmetros
    // Ela simplesmente retorna TODAS as mensagens
}