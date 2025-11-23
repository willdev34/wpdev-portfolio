// ====================================
// Título: ProjectRepository.cs (REFATORADO)
// Descrição: Implementação do Repository usando EF Core - Design Editorial
// Autor: Will
// Empresa: WpDev
// Data: 23/11/2024
// ====================================

using Microsoft.EntityFrameworkCore;
using Portfolio.Application.Interfaces;
using Portfolio.Domain.Entities;
using Portfolio.Infrastructure.Data;

namespace Portfolio.Infrastructure.Repositories;

/// <summary>
/// Implementação do Repository para Project usando Entity Framework Core
/// Responsável por todas as operações de acesso a dados da entidade Project
/// </summary>
public class ProjectRepository : IProjectRepository
{
    private readonly PortfolioDbContext _context;

    public ProjectRepository(PortfolioDbContext context)
    {
        _context = context;
    }

    // ==========================================
    // CONSULTAS (QUERIES)
    // ==========================================

    /// <summary>
    /// Busca TODOS os projetos ativos, ordenados por data de criação (mais recentes primeiro)
    /// </summary>
    public async Task<IEnumerable<Project>> GetAllAsync()
    {
        return await _context.Projects
            .Where(p => p.IsActive)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();
    }

    /// <summary>
    /// Busca um projeto específico por ID
    /// </summary>
    public async Task<Project?> GetByIdAsync(Guid id)
    {
        return await _context.Projects
            .FirstOrDefaultAsync(p => p.Id == id && p.IsActive);
    }

    /// <summary>
    /// Busca apenas projetos em destaque, ordenados por data (mais recentes primeiro)
    /// </summary>
    public async Task<IEnumerable<Project>> GetFeaturedAsync()
    {
        return await _context.Projects
            .Where(p => p.IsActive && p.IsFeatured)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();
    }

    /// <summary>
    /// Busca projetos de um ano específico
    /// </summary>
    public async Task<IEnumerable<Project>> GetByYearAsync(int year)
    {
        return await _context.Projects
            .Where(p => p.IsActive && p.Year == year)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();
    }

    /// <summary>
    /// Busca projetos que contêm uma tecnologia específica
    /// Usa EF.Functions.ILike para busca case-insensitive
    /// </summary>
    public async Task<IEnumerable<Project>> GetByTechnologyAsync(string technology)
    {
        return await _context.Projects
            .Where(p => p.IsActive && p.Technologies.Any(t => EF.Functions.ILike(t, $"%{technology}%")))
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();
    }

    // ==========================================
    // COMANDOS (MUTATIONS)
    // ==========================================

    /// <summary>
    /// Adiciona um novo projeto ao contexto
    /// IMPORTANTE: Não salva automaticamente, precisa chamar SaveChangesAsync()
    /// </summary>
    public async Task<Project> AddAsync(Project project)
    {
        // Gera um novo ID
        project.Id = Guid.NewGuid();
        
        // Seta a data de criação
        project.CreatedAt = DateTime.UtcNow;
        
        // Garante que está ativo
        project.IsActive = true;

        await _context.Projects.AddAsync(project);
        
        return project;
    }

    /// <summary>
    /// Atualiza um projeto existente
    /// IMPORTANTE: Não salva automaticamente, precisa chamar SaveChangesAsync()
    /// </summary>
    public async Task UpdateAsync(Project project)
    {
        // Seta a data de atualização
        project.UpdatedAt = DateTime.UtcNow;

        _context.Projects.Update(project);
        
        await Task.CompletedTask; // Para manter assinatura async
    }

    /// <summary>
    /// Soft delete - marca o projeto como inativo ao invés de deletar do banco
    /// IMPORTANTE: Não salva automaticamente, precisa chamar SaveChangesAsync()
    /// </summary>
    public async Task DeleteAsync(Guid id)
    {
        var project = await _context.Projects.FindAsync(id);
        
        if (project != null)
        {
            project.IsActive = false;
            project.UpdatedAt = DateTime.UtcNow;
            _context.Projects.Update(project);
        }
    }

    /// <summary>
    /// Verifica se um projeto existe no banco
    /// </summary>
    public async Task<bool> ExistsAsync(Guid id)
    {
        return await _context.Projects.AnyAsync(p => p.Id == id && p.IsActive);
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