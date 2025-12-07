// ====================================
// Título: ITimelineEventRepository.cs
// Descrição: Interface do Repository para TimelineEvent
// Autor: Will
// Empresa: WpDev
// Data: 29/11/2024
// ====================================

using Portfolio.Domain.Entities;

namespace Portfolio.Application.Interfaces;

/// <summary>
/// Interface do Repository para a entidade TimelineEvent
/// Define o CONTRATO de quais operações de acesso a dados devem existir
/// A implementação fica na camada Infrastructure
/// </summary>
public interface ITimelineEventRepository
{
    // ==========================================
    // CONSULTAS (QUERIES)
    // ==========================================
    
    /// <summary>
    /// Busca TODOS os eventos da timeline
    /// Ordenados por Order (crescente) - eventos mais antigos primeiro
    /// </summary>
    /// <returns>Lista de todos os eventos</returns>
    Task<IEnumerable<TimelineEvent>> GetAllAsync();
    
    /// <summary>
    /// Busca um evento por ID
    /// </summary>
    /// <param name="id">ID do evento</param>
    /// <returns>Evento encontrado ou null</returns>
    Task<TimelineEvent?> GetByIdAsync(Guid id);
    
    /// <summary>
    /// Busca apenas eventos VISÍVEIS (IsVisible = true)
    /// Ordenados por Order (crescente)
    /// Usado para exibir a timeline no site público
    /// </summary>
    /// <returns>Lista de eventos visíveis</returns>
    Task<IEnumerable<TimelineEvent>> GetVisibleAsync();
    
    /// <summary>
    /// Busca eventos por tipo
    /// </summary>
    /// <param name="type">Tipo do evento (Education, Work, Project, etc.)</param>
    /// <returns>Lista de eventos do tipo especificado</returns>
    Task<IEnumerable<TimelineEvent>> GetByTypeAsync(TimelineEventType type);
    
    /// <summary>
    /// Busca eventos por ano
    /// </summary>
    /// <param name="year">Ano do evento</param>
    /// <returns>Lista de eventos do ano especificado</returns>
    Task<IEnumerable<TimelineEvent>> GetByYearAsync(int year);
    
    // ==========================================
    // COMANDOS (MUTATIONS)
    // ==========================================
    
    /// <summary>
    /// Adiciona um novo evento ao banco
    /// </summary>
    /// <param name="timelineEvent">Evento a ser adicionado</param>
    /// <returns>Evento adicionado com ID gerado</returns>
    Task<TimelineEvent> AddAsync(TimelineEvent timelineEvent);
    
    /// <summary>
    /// Atualiza um evento existente
    /// </summary>
    /// <param name="timelineEvent">Evento com dados atualizados</param>
    Task UpdateAsync(TimelineEvent timelineEvent);
    
    /// <summary>
    /// Deleta um evento (soft delete - apenas marca como invisível)
    /// DIFERENTE do BlogPost que usa hard delete
    /// </summary>
    /// <param name="id">ID do evento a deletar</param>
    Task DeleteAsync(Guid id);
    
    /// <summary>
    /// Verifica se um evento existe
    /// </summary>
    /// <param name="id">ID do evento</param>
    /// <returns>True se existir, False caso contrário</returns>
    Task<bool> ExistsAsync(Guid id);
    
    // ==========================================
    // PERSISTÊNCIA
    // ==========================================
    
    /// <summary>
    /// Salva todas as mudanças no banco de dados
    /// Usado após AddAsync, UpdateAsync, DeleteAsync
    /// </summary>
    Task<int> SaveChangesAsync();
}