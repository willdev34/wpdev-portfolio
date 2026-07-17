// ====================================
// Título: EmailService.cs
// Descrição: Envio de emails via SMTP (MailDev local, Resend em produção)
// ====================================

using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MimeKit;
using Portfolio.Application.Interfaces;
using Portfolio.Domain.Entities;

namespace Portfolio.Infrastructure.Services;

public class EmailService : IEmailService
{
    private readonly IConfiguration _config;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IConfiguration config, ILogger<EmailService> logger)
    {
        _config = config;
        _logger = logger;
    }

    public async Task SendContactNotificationAsync(ContactMessage message, CancellationToken cancellationToken = default)
    {
        try
        {
            var host = _config["Smtp:Host"];
            var user = _config["Smtp:User"];
            var password = _config["Smtp:Password"];
            var from = _config["Smtp:From"];
            var notifyTo = _config["Smtp:NotifyTo"];
            var port = int.TryParse(_config["Smtp:Port"], out var parsedPort) ? parsedPort : 587;

            if (string.IsNullOrWhiteSpace(host) || string.IsNullOrWhiteSpace(from) || string.IsNullOrWhiteSpace(notifyTo))
            {
                _logger.LogWarning("[EmailService] Configuracao de SMTP incompleta, notificacao nao enviada.");
                return;
            }

            var typeLabel = message.Type switch
            {
                ContactMessageType.JobOpportunity => "Oportunidade de emprego",
                ContactMessageType.Freelance => "Freelance",
                ContactMessageType.Partnership => "Parceria",
                ContactMessageType.Other => "Outro",
                _ => "Geral"
            };

            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(from));
            email.To.Add(MailboxAddress.Parse(notifyTo));
            email.ReplyTo.Add(new MailboxAddress(message.Name, message.Email));
            email.Subject = $"[WPDev Portfolio] Nova mensagem: {message.Subject}";

            email.Body = new TextPart("html")
            {
                Text = $"""
                    <div style="font-family: Arial, sans-serif; color: #0D1C24; max-width: 560px;">
                        <h2 style="font-family: Arial, sans-serif;">Nova mensagem de contato</h2>
                        <p><strong>Tipo:</strong> {typeLabel}</p>
                        <p><strong>De:</strong> {message.Name} ({message.Email})</p>
                        <p><strong>Assunto:</strong> {message.Subject}</p>
                        <hr style="border: none; border-top: 1px solid #A3A9AB; margin: 16px 0;" />
                        <p style="white-space: pre-wrap;">{message.Message}</p>
                        <hr style="border: none; border-top: 1px solid #A3A9AB; margin: 16px 0;" />
                        <p style="color: #A3A9AB; font-size: 0.85rem;">Responda direto este email para falar com quem enviou.</p>
                    </div>
                    """
            };

            using var smtp = new SmtpClient
            {
                Timeout = 8000 // 8 segundos no maximo por operacao (connect, auth, send)
            };
            var secureOption = port == 465 ? SecureSocketOptions.SslOnConnect : SecureSocketOptions.StartTlsWhenAvailable;

            await smtp.ConnectAsync(host, port, secureOption, cancellationToken);

            if (!string.IsNullOrWhiteSpace(user))
            {
                await smtp.AuthenticateAsync(user, password, cancellationToken);
            }

            await smtp.SendAsync(email, cancellationToken);
            await smtp.DisconnectAsync(true, cancellationToken);

            _logger.LogInformation("[EmailService] Notificacao enviada para {NotifyTo}.", notifyTo);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[EmailService] Falha ao enviar notificacao de contato.");
        }
    }
}