// ====================================
// Título: IEmailService.cs
// Descrição: Contrato para envio de notificações por email
// ====================================

using Portfolio.Domain.Entities;

namespace Portfolio.Application.Interfaces;

public interface IEmailService
{
    /// <summary>
    /// Envia uma notificação por email avisando que uma nova mensagem de contato chegou.
    /// Implementações não devem lançar exceção em caso de falha: o envio de email é uma
    /// notificação auxiliar, e uma falha aqui nunca deve impedir o fluxo principal
    /// (a mensagem de contato já foi salva no banco antes desta chamada).
    /// </summary>
    Task SendContactNotificationAsync(ContactMessage message, CancellationToken cancellationToken = default);
}