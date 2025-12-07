// ====================================
// Título: IContactMessageRepository.cs
// Descrição: Interface do Repository para ContactMessage
// Autor: Will
// Empresa: WpDev
// Data: 06/12/2024
// ====================================

using Portfolio.Domain.Entities;

namespace Portfolio.Application.Interfaces;

/// <summary>
/// Interface do Repository para a entidade ContactMessage
/// Define o CONTRATO de quais operações de acesso a dados devem existir
/// A implementação fica na camada Infrastructure
/// </summary>
public interface IContactMessageRepository
{
    // ==========================================
    // CONSULTAS (QUERIES)
    // ==========================================
    
    /// <summary>
    /// Busca TODAS as mensagens de contato
    /// Ordenadas por CreatedAt (mais recentes primeiro)
    /// </summary>
    /// <returns>Lista de todas as mensagens</returns>
    Task<IEnumerable<ContactMessage>> GetAllAsync();
    
    /// <summary>
    /// Busca uma mensagem por ID
    /// </summary>
    /// <param name="id">ID da mensagem</param>
    /// <returns>Mensagem encontrada ou null</returns>
    Task<ContactMessage?> GetByIdAsync(Guid id);
    
    /// <summary>
    /// Busca mensagens por STATUS
    /// Exemplo: GetByStatusAsync(ContactMessageStatus.New)
    /// Ordenadas por CreatedAt (mais recentes primeiro)
    /// </summary>
    /// <param name="status">Status da mensagem</param>
    /// <returns>Lista de mensagens com o status especificado</returns>
    Task<IEnumerable<ContactMessage>> GetByStatusAsync(ContactMessageStatus status);
    
    /// <summary>
    /// Busca mensagens por TIPO
    /// Exemplo: GetByTypeAsync(ContactMessageType.JobOpportunity)
    /// Ordenadas por CreatedAt (mais recentes primeiro)
    /// </summary>
    /// <param name="type">Tipo da mensagem</param>
    /// <returns>Lista de mensagens do tipo especificado</returns>
    Task<IEnumerable<ContactMessage>> GetByTypeAsync(ContactMessageType type);
    
    /// <summary>
    /// Busca apenas mensagens NÃO LIDAS (Status = New)
    /// Útil para exibir contador de mensagens novas no admin
    /// Ordenadas por CreatedAt (mais recentes primeiro)
    /// </summary>
    /// <returns>Lista de mensagens não lidas</returns>
    Task<IEnumerable<ContactMessage>> GetUnreadAsync();
    
    // ==========================================
    // COMANDOS (MUTATIONS)
    // ==========================================
    
    /// <summary>
    /// Adiciona uma nova mensagem ao banco
    /// </summary>
    /// <param name="contactMessage">Mensagem a ser adicionada</param>
    /// <returns>Mensagem adicionada com ID gerado</returns>
    Task<ContactMessage> AddAsync(ContactMessage contactMessage);
    
    /// <summary>
    /// Atualiza uma mensagem existente
    /// Usado principalmente para mudar Status, adicionar ResponseMessage
    /// </summary>
    /// <param name="contactMessage">Mensagem com dados atualizados</param>
    Task UpdateAsync(ContactMessage contactMessage);
    
    /// <summary>
    /// Verifica se uma mensagem existe
    /// </summary>
    /// <param name="id">ID da mensagem</param>
    /// <returns>True se existir, False caso contrário</returns>
    Task<bool> ExistsAsync(Guid id);
    
    // ==========================================
    // PERSISTÊNCIA
    // ==========================================
    
    /// <summary>
    /// Salva todas as mudanças no banco de dados
    /// Usado após AddAsync, UpdateAsync
    /// </summary>
    Task<int> SaveChangesAsync();
    
    // ==========================================
    // NOTA IMPORTANTE
    // ==========================================
    // ContactMessage NÃO tem método DeleteAsync()
    // As mensagens NUNCA são deletadas (mantidas para auditoria)
    // Use Status = Archived ou Spam para "esconder" mensagens
}