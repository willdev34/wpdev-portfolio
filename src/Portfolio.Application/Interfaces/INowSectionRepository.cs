// ====================================
// Título: INowSectionRepository.cs
// Descrição: Interface do Repository para NowSection
// Autor: Will
// Empresa: WpDev
// Data: 06/12/2024
// ====================================

using Portfolio.Domain.Entities;

namespace Portfolio.Application.Interfaces;

/// <summary>
/// Interface do Repository para a entidade NowSection
/// Define o CONTRATO de quais operações de acesso a dados devem existir
/// A implementação fica na camada Infrastructure
/// </summary>
public interface INowSectionRepository
{
    // ==========================================
    // CONSULTAS (QUERIES)
    // ==========================================
    
    /// <summary>
    /// Busca TODAS as seções Now
    /// Ordenadas por UpdatedAt (mais recente primeiro)
    /// </summary>
    /// <returns>Lista de todas as seções</returns>
    Task<IEnumerable<NowSection>> GetAllAsync();
    
    /// <summary>
    /// Busca uma seção por ID
    /// </summary>
    /// <param name="id">ID da seção</param>
    /// <returns>Seção encontrada ou null</returns>
    Task<NowSection?> GetByIdAsync(Guid id);
    
    /// <summary>
    /// Busca a seção ATIVA (IsActive = true)
    /// IMPORTANTE: Apenas 1 seção pode estar ativa por vez
    /// Usada para exibir no site público
    /// </summary>
    /// <returns>Seção ativa ou null</returns>
    Task<NowSection?> GetActiveAsync();
    
    // ==========================================
    // COMANDOS (MUTATIONS)
    // ==========================================
    
    /// <summary>
    /// Adiciona uma nova seção ao banco
    /// </summary>
    /// <param name="nowSection">Seção a ser adicionada</param>
    /// <returns>Seção adicionada com ID gerado</returns>
    Task<NowSection> AddAsync(NowSection nowSection);
    
    /// <summary>
    /// Atualiza uma seção existente
    /// </summary>
    /// <param name="nowSection">Seção com dados atualizados</param>
    Task UpdateAsync(NowSection nowSection);
    
    /// <summary>
    /// DESATIVA TODAS as seções (IsActive = false)
    /// IMPORTANTE: Usado antes de ativar uma nova seção
    /// Garante que apenas 1 seção esteja ativa por vez (business rule)
    /// </summary>
    Task DeactivateAllAsync();
    
    /// <summary>
    /// Deleta uma seção (soft delete - marca como inativa)
    /// </summary>
    /// <param name="id">ID da seção a deletar</param>
    Task DeleteAsync(Guid id);
    
    /// <summary>
    /// Verifica se uma seção existe
    /// </summary>
    /// <param name="id">ID da seção</param>
    /// <returns>True se existir, False caso contrário</returns>
    Task<bool> ExistsAsync(Guid id);
    
    // ==========================================
    // PERSISTÊNCIA
    // ==========================================
    
    /// <summary>
    /// Salva todas as mudanças no banco de dados
    /// Usado após AddAsync, UpdateAsync, DeleteAsync, DeactivateAllAsync
    /// </summary>
    Task<int> SaveChangesAsync();
}