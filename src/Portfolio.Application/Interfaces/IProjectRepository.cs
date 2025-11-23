// ====================================
// Título: IProjectRepository.cs (REFATORADO)
// Descrição: Interface do Repository - Design Editorial
// Autor: Will
// Empresa: WpDev
// Data: 23/11/2024
// ====================================

using Portfolio.Domain.Entities;

namespace Portfolio.Application.Interfaces;

/// <summary>
/// Interface do Repository para a entidade Project
/// Define o CONTRATO de quais operações de acesso a dados devem existir
/// A implementação fica na camada Infrastructure
/// </summary>
public interface IProjectRepository
{
    // ==========================================
    // CONSULTAS (QUERIES)
    // ==========================================
    
    /// <summary>
    /// Busca TODOS os projetos ativos
    /// </summary>
    /// <returns>Lista de todos os projetos</returns>
    Task<IEnumerable<Project>> GetAllAsync();
    
    /// <summary>
    /// Busca um projeto por ID
    /// </summary>
    /// <param name="id">ID do projeto</param>
    /// <returns>Projeto encontrado ou null</returns>
    Task<Project?> GetByIdAsync(Guid id);
    
    /// <summary>
    /// Busca apenas projetos em destaque (IsFeatured = true)
    /// </summary>
    /// <returns>Lista de projetos em destaque</returns>
    Task<IEnumerable<Project>> GetFeaturedAsync();
    
    /// <summary>
    /// Busca projetos por ano
    /// </summary>
    /// <param name="year">Ano do projeto</param>
    /// <returns>Lista de projetos do ano especificado</returns>
    Task<IEnumerable<Project>> GetByYearAsync(int year);
    
    /// <summary>
    /// Busca projetos por tecnologia
    /// </summary>
    /// <param name="technology">Nome da tecnologia (ex: "React", "C#")</param>
    /// <returns>Lista de projetos que usam a tecnologia</returns>
    Task<IEnumerable<Project>> GetByTechnologyAsync(string technology);
    
    // ==========================================
    // COMANDOS (MUTATIONS)
    // ==========================================
    
    /// <summary>
    /// Adiciona um novo projeto ao banco
    /// </summary>
    /// <param name="project">Projeto a ser adicionado</param>
    /// <returns>Projeto adicionado com ID gerado</returns>
    Task<Project> AddAsync(Project project);
    
    /// <summary>
    /// Atualiza um projeto existente
    /// </summary>
    /// <param name="project">Projeto com dados atualizados</param>
    Task UpdateAsync(Project project);
    
    /// <summary>
    /// Deleta um projeto (soft delete - apenas marca como inativo)
    /// </summary>
    /// <param name="id">ID do projeto a deletar</param>
    Task DeleteAsync(Guid id);
    
    /// <summary>
    /// Verifica se um projeto existe
    /// </summary>
    /// <param name="id">ID do projeto</param>
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