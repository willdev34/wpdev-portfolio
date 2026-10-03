// ====================================
// Título: TimelineEventRepository.cs
// Descrição: Implementação do Repository usando EF Core para TimelineEvent
// ====================================

using Microsoft.EntityFrameworkCore;
using Portfolio.Application.Interfaces;
using Portfolio.Domain.Entities;
using Portfolio.Infrastructure.Data;

namespace Portfolio.Infrastructure.Repositories;

/// <summary>
/// Implementação do Repository para TimelineEvent usando Entity Framework Core
/// Responsável por todas as operações de acesso a dados da entidade TimelineEvent
/// </summary>
public class TimelineEventRepository : ITimelineEventRepository
{
    private readonly PortfolioDbContext _context;

    public TimelineEventRepository(PortfolioDbContext context)
    {
        _context = context;
    }

    // ==========================================
    // CONSULTAS (QUERIES)
    // ==========================================

    /// <summary>
    /// Busca TODOS os eventos da timeline
    /// Ordenados por Order (crescente) - ordem cronológica
    /// </summary>
    public async Task<IEnumerable<TimelineEvent>> GetAllAsync()
    {
        return await OrderChronologically(
                _context.TimelineEvents
                    .Include(e => e.Skills.OrderBy(s => s.Order))
                    .Where(e => e.IsVisible))
            .ToListAsync();
    }

    /// <summary>
    /// Busca um evento específico por ID
    /// </summary>
    public async Task<TimelineEvent?> GetByIdAsync(Guid id)
    {
        return await _context.TimelineEvents
            .Include(e => e.Skills.OrderBy(s => s.Order))
            .FirstOrDefaultAsync(e => e.Id == id);
    }

    /// <summary>
    /// Busca apenas eventos VISÍVEIS (IsVisible = true)
    /// Ordenados por Order (crescente)
    /// Usado para exibir a timeline no site público
    /// </summary>
    public async Task<IEnumerable<TimelineEvent>> GetVisibleAsync()
    {
        return await OrderChronologically(
                _context.TimelineEvents
                    .Include(e => e.Skills.OrderBy(s => s.Order))
                    .Where(e => e.IsVisible))
            .ToListAsync();
    }

    /// <summary>
    /// Busca eventos por tipo
    /// Ordenados por Order (crescente)
    /// </summary>
    public async Task<IEnumerable<TimelineEvent>> GetByTypeAsync(TimelineEventType type)
    {
        return await OrderChronologically(
                _context.TimelineEvents
                    .Include(e => e.Skills.OrderBy(s => s.Order))
                    .Where(e => e.IsVisible && e.Type == type))
            .ToListAsync();
    }

    /// <summary>
    /// Busca eventos por ano
    /// Ordenados por Order (crescente)
    /// </summary>
    public async Task<IEnumerable<TimelineEvent>> GetByYearAsync(int year)
    {
        return await OrderChronologically(
                _context.TimelineEvents
                    .Include(e => e.Skills.OrderBy(s => s.Order))
                    .Where(e => e.IsVisible && e.Date.Year == year))
            .ToListAsync();
    }

    // ==========================================
    // ORDENAÇÃO (único lugar)
    // ==========================================

    /// <summary>
    /// Ordem cronológica da timeline, usada por TODAS as consultas (pública e admin):
    /// 1. data inicial crescente
    /// 2. data final crescente, com evento SEM data final (em andamento) depois dos que já terminaram
    /// 3. CreatedAt crescente
    /// O campo Order não participa mais da ordenação
    /// </summary>
    private static IQueryable<TimelineEvent> OrderChronologically(IQueryable<TimelineEvent> query)
    {
        return query
            .OrderBy(e => e.Date)
            .ThenBy(e => e.EndDate == null)
            .ThenBy(e => e.EndDate)
            .ThenBy(e => e.CreatedAt);
    }

    // ==========================================
    // COMANDOS (MUTATIONS)
    // ==========================================

    /// <summary>
    /// Adiciona um novo evento ao contexto
    /// IMPORTANTE: Não salva automaticamente, precisa chamar SaveChangesAsync()
    /// </summary>
    public async Task<TimelineEvent> AddAsync(TimelineEvent timelineEvent)
    {
        // Gera um novo ID
        timelineEvent.Id = Guid.NewGuid();
        
        // Seta a data de criação
        timelineEvent.CreatedAt = DateTime.UtcNow;
        
        // Garante que está visível por padrão
        if (!timelineEvent.IsVisible)
        {
            timelineEvent.IsVisible = true;
        }

        await _context.TimelineEvents.AddAsync(timelineEvent);
        
        return timelineEvent;
    }

    /// <summary>
    /// Atualiza um evento existente
    /// IMPORTANTE: Não salva automaticamente, precisa chamar SaveChangesAsync()
    /// </summary>
    public async Task UpdateAsync(TimelineEvent timelineEvent)
    {
        // Seta a data de atualização
        timelineEvent.UpdatedAt = DateTime.UtcNow;

        _context.TimelineEvents.Update(timelineEvent);
        
        await Task.CompletedTask; // Para manter assinatura async
    }

    /// <summary>
    /// Soft delete - marca o evento como invisível ao invés de deletar fisicamente
    /// DIFERENTE do BlogPost que usa hard delete
    /// </summary>
    public async Task ReplaceSkillsAsync(TimelineEvent timelineEvent, IEnumerable<TimelineEventSkill> skills)
    {
        // Remove as habilidades atuais (carregadas pelo GetByIdAsync) e insere as novas.
        // Add explícito: os Ids vêm vazios e o EF gera, então entram como Added (não como Modified)
        _context.TimelineEventSkills.RemoveRange(timelineEvent.Skills.ToList());

        foreach (var skill in skills)
        {
            skill.TimelineEventId = timelineEvent.Id;
            _context.TimelineEventSkills.Add(skill);
        }

        await Task.CompletedTask; // Para manter assinatura async
    }

    public async Task DeleteAsync(Guid id)
    {
        var timelineEvent = await _context.TimelineEvents.FindAsync(id);
        
        if (timelineEvent != null)
        {
            timelineEvent.IsVisible = false;
            timelineEvent.UpdatedAt = DateTime.UtcNow;
            _context.TimelineEvents.Update(timelineEvent);
        }
    }

    /// <summary>
    /// Verifica se um evento existe no banco
    /// </summary>
    public async Task<bool> ExistsAsync(Guid id)
    {
        return await _context.TimelineEvents.AnyAsync(e => e.Id == id);
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
}