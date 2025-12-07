// ====================================
// Título: ContactMessageRepository.cs
// Descrição: Implementação do Repository usando EF Core para ContactMessage
// Autor: Will
// Empresa: WpDev
// Data: 06/12/2024
// ====================================

using Microsoft.EntityFrameworkCore;
using Portfolio.Application.Interfaces;
using Portfolio.Domain.Entities;
using Portfolio.Infrastructure.Data;

namespace Portfolio.Infrastructure.Repositories;

/// <summary>
/// Implementação do Repository para ContactMessage usando Entity Framework Core
/// Responsável por todas as operações de acesso a dados da entidade ContactMessage
/// IMPORTANTE: ContactMessage NÃO deleta (mantém tudo para auditoria)
/// </summary>
public class ContactMessageRepository : IContactMessageRepository
{
    private readonly PortfolioDbContext _context;

    public ContactMessageRepository(PortfolioDbContext context)
    {
        _context = context;
    }

    // ==========================================
    // CONSULTAS (QUERIES)
    // ==========================================

    /// <summary>
    /// Busca TODAS as mensagens de contato
    /// Ordenadas por CreatedAt (mais recentes primeiro)
    /// </summary>
    public async Task<IEnumerable<ContactMessage>> GetAllAsync()
    {
        return await _context.ContactMessages
            .OrderByDescending(m => m.CreatedAt)
            .ToListAsync();
    }

    /// <summary>
    /// Busca uma mensagem específica por ID
    /// </summary>
    public async Task<ContactMessage?> GetByIdAsync(Guid id)
    {
        return await _context.ContactMessages
            .FirstOrDefaultAsync(m => m.Id == id);
    }

    /// <summary>
    /// Busca mensagens por STATUS
    /// Exemplo: GetByStatusAsync(ContactMessageStatus.New)
    /// Ordenadas por CreatedAt (mais recentes primeiro)
    /// </summary>
    public async Task<IEnumerable<ContactMessage>> GetByStatusAsync(ContactMessageStatus status)
    {
        return await _context.ContactMessages
            .Where(m => m.Status == status)
            .OrderByDescending(m => m.CreatedAt)
            .ToListAsync();
    }

    /// <summary>
    /// Busca mensagens por TIPO
    /// Exemplo: GetByTypeAsync(ContactMessageType.JobOpportunity)
    /// Ordenadas por CreatedAt (mais recentes primeiro)
    /// </summary>
    public async Task<IEnumerable<ContactMessage>> GetByTypeAsync(ContactMessageType type)
    {
        return await _context.ContactMessages
            .Where(m => m.Type == type)
            .OrderByDescending(m => m.CreatedAt)
            .ToListAsync();
    }

    /// <summary>
    /// Busca apenas mensagens NÃO LIDAS (Status = New)
    /// Útil para exibir contador no admin
    /// Ordenadas por CreatedAt (mais recentes primeiro)
    /// </summary>
    public async Task<IEnumerable<ContactMessage>> GetUnreadAsync()
    {
        return await _context.ContactMessages
            .Where(m => m.Status == ContactMessageStatus.New)
            .OrderByDescending(m => m.CreatedAt)
            .ToListAsync();
    }

    // ==========================================
    // COMANDOS (MUTATIONS)
    // ==========================================

    /// <summary>
    /// Adiciona uma nova mensagem ao contexto
    /// IMPORTANTE: Não salva automaticamente, precisa chamar SaveChangesAsync()
    /// </summary>
    public async Task<ContactMessage> AddAsync(ContactMessage contactMessage)
    {
        // Gera um novo ID
        contactMessage.Id = Guid.NewGuid();
        
        // Seta a data de criação
        contactMessage.CreatedAt = DateTime.UtcNow;
        
        // Garante que Status começa como New
        contactMessage.Status = ContactMessageStatus.New;

        await _context.ContactMessages.AddAsync(contactMessage);
        
        return contactMessage;
    }

    /// <summary>
    /// Atualiza uma mensagem existente
    /// Usado principalmente para mudar Status e adicionar ResponseMessage
    /// IMPORTANTE: Não salva automaticamente, precisa chamar SaveChangesAsync()
    /// </summary>
    public async Task UpdateAsync(ContactMessage contactMessage)
    {
        // Seta a data de atualização
        contactMessage.UpdatedAt = DateTime.UtcNow;

        _context.ContactMessages.Update(contactMessage);
        
        await Task.CompletedTask; // Para manter assinatura async
    }

    /// <summary>
    /// Verifica se uma mensagem existe no banco
    /// </summary>
    public async Task<bool> ExistsAsync(Guid id)
    {
        return await _context.ContactMessages.AnyAsync(m => m.Id == id);
    }

    // ==========================================
    // PERSISTÊNCIA
    // ==========================================

    /// <summary>
    /// Salva todas as mudanças pendentes no banco de dados
    /// Retorna o número de registros afetados
    /// </summary>
    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }
    
    // ==========================================
    // NOTA: SEM DELETE
    // ==========================================
    // ContactMessage NÃO tem método DeleteAsync()
    // Todas mensagens são mantidas para auditoria
    // Use Status = Archived ou Spam para "esconder"
}