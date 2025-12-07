// ====================================
// Título: NowSectionRepository.cs
// Descrição: Implementação do Repository usando EF Core para NowSection
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
/// Implementação do Repository para NowSection usando Entity Framework Core
/// Responsável por todas as operações de acesso a dados da entidade NowSection
/// IMPORTANTE: Apenas 1 seção pode estar ativa por vez (business rule)
/// </summary>
public class NowSectionRepository : INowSectionRepository
{
    private readonly PortfolioDbContext _context;

    public NowSectionRepository(PortfolioDbContext context)
    {
        _context = context;
    }

    // ==========================================
    // CONSULTAS (QUERIES)
    // ==========================================

    /// <summary>
    /// Busca TODAS as seções Now
    /// Ordenadas por UpdatedAt (mais recente primeiro)
    /// </summary>
    public async Task<IEnumerable<NowSection>> GetAllAsync()
    {
        return await _context.NowSections
            .OrderByDescending(n => n.UpdatedAt)
            .ToListAsync();
    }

    /// <summary>
    /// Busca uma seção específica por ID
    /// </summary>
    public async Task<NowSection?> GetByIdAsync(Guid id)
    {
        return await _context.NowSections
            .FirstOrDefaultAsync(n => n.Id == id);
    }

    /// <summary>
    /// Busca a seção ATIVA (IsActive = true)
    /// IMPORTANTE: Apenas 1 seção pode estar ativa por vez
    /// Usada para exibir no site público
    /// </summary>
    public async Task<NowSection?> GetActiveAsync()
    {
        return await _context.NowSections
            .FirstOrDefaultAsync(n => n.IsActive);
    }

    // ==========================================
    // COMANDOS (MUTATIONS)
    // ==========================================

    /// <summary>
    /// Adiciona uma nova seção ao contexto
    /// IMPORTANTE: Não salva automaticamente, precisa chamar SaveChangesAsync()
    /// </summary>
    public async Task<NowSection> AddAsync(NowSection nowSection)
    {
        // Gera um novo ID
        nowSection.Id = Guid.NewGuid();
        
        // Seta as datas de criação e atualização
        nowSection.CreatedAt = DateTime.UtcNow;
        nowSection.UpdatedAt = DateTime.UtcNow;
        
        // Garante que está ativo por padrão
        nowSection.IsActive = true;

        await _context.NowSections.AddAsync(nowSection);
        
        return nowSection;
    }

    /// <summary>
    /// Atualiza uma seção existente
    /// IMPORTANTE: Não salva automaticamente, precisa chamar SaveChangesAsync()
    /// </summary>
    public async Task UpdateAsync(NowSection nowSection)
    {
        // Seta a data de atualização
        nowSection.UpdatedAt = DateTime.UtcNow;

        _context.NowSections.Update(nowSection);
        
        await Task.CompletedTask; // Para manter assinatura async
    }

    /// <summary>
    /// DESATIVA TODAS as seções (IsActive = false)
    /// IMPORTANTE: Usado antes de ativar uma nova seção
    /// Garante que apenas 1 seção esteja ativa por vez (business rule)
    /// </summary>
    public async Task DeactivateAllAsync()
    {
        var allSections = await _context.NowSections.ToListAsync();
        
        foreach (var section in allSections)
        {
            section.IsActive = false;
            section.UpdatedAt = DateTime.UtcNow;
        }
        
        _context.NowSections.UpdateRange(allSections);
    }

    /// <summary>
    /// Soft delete - marca a seção como inativa ao invés de deletar fisicamente
    /// </summary>
    public async Task DeleteAsync(Guid id)
    {
        var nowSection = await _context.NowSections.FindAsync(id);
        
        if (nowSection != null)
        {
            nowSection.IsActive = false;
            nowSection.UpdatedAt = DateTime.UtcNow;
            _context.NowSections.Update(nowSection);
        }
    }

    /// <summary>
    /// Verifica se uma seção existe no banco
    /// </summary>
    public async Task<bool> ExistsAsync(Guid id)
    {
        return await _context.NowSections.AnyAsync(n => n.Id == id);
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