// ====================================
// Título: IBlogPostRepository.cs
// Descrição: Interface do Repository para BlogPost
// Autor: Will
// Empresa: WpDev
// Data: 23/11/2024
// ====================================

using Portfolio.Domain.Entities;

namespace Portfolio.Application.Interfaces;

/// <summary>
/// Interface do Repository para a entidade BlogPost
/// Define o CONTRATO de quais operações de acesso a dados devem existir
/// A implementação fica na camada Infrastructure
/// </summary>
public interface IBlogPostRepository
{
    // ==========================================
    // CONSULTAS (QUERIES)
    // ==========================================
    
    /// <summary>
    /// Busca TODOS os posts (publicados e rascunhos)
    /// </summary>
    /// <returns>Lista de todos os posts</returns>
    Task<IEnumerable<BlogPost>> GetAllAsync();
    
    /// <summary>
    /// Busca um post por ID
    /// </summary>
    /// <param name="id">ID do post</param>
    /// <returns>Post encontrado ou null</returns>
    Task<BlogPost?> GetByIdAsync(Guid id);
    
    /// <summary>
    /// Busca um post por Slug (URL amigável)
    /// Exemplo: "como-aprender-dotnet" ao invés de usar o ID
    /// </summary>
    /// <param name="slug">Slug do post</param>
    /// <returns>Post encontrado ou null</returns>
    Task<BlogPost?> GetBySlugAsync(string slug);
    
    /// <summary>
    /// Busca apenas posts PUBLICADOS (IsPublished = true)
    /// Ordenados por data de publicação (mais recentes primeiro)
    /// </summary>
    /// <returns>Lista de posts publicados</returns>
    Task<IEnumerable<BlogPost>> GetPublishedAsync();
    
    /// <summary>
    /// Busca posts em DESTAQUE (IsFeatured = true)
    /// </summary>
    /// <returns>Lista de posts em destaque</returns>
    Task<IEnumerable<BlogPost>> GetFeaturedAsync();
    
    /// <summary>
    /// Busca posts por tag
    /// </summary>
    /// <param name="tag">Nome da tag (ex: "C#", "Blazor")</param>
    /// <returns>Lista de posts que contêm a tag</returns>
    Task<IEnumerable<BlogPost>> GetByTagAsync(string tag);
    
    // ==========================================
    // COMANDOS (MUTATIONS)
    // ==========================================
    
    /// <summary>
    /// Adiciona um novo post ao banco
    /// </summary>
    /// <param name="blogPost">Post a ser adicionado</param>
    /// <returns>Post adicionado com ID gerado</returns>
    Task<BlogPost> AddAsync(BlogPost blogPost);
    
    /// <summary>
    /// Atualiza um post existente
    /// </summary>
    /// <param name="blogPost">Post com dados atualizados</param>
    Task UpdateAsync(BlogPost blogPost);
    
    /// <summary>
    /// Deleta um post fisicamente do banco
    /// ATENÇÃO: BlogPost NÃO usa soft delete (ao contrário de Project)
    /// </summary>
    /// <param name="id">ID do post a deletar</param>
    Task DeleteAsync(Guid id);
    
    /// <summary>
    /// Incrementa o contador de visualizações de um post
    /// </summary>
    /// <param name="id">ID do post</param>
    Task IncrementViewCountAsync(Guid id);
    
    /// <summary>
    /// Verifica se um post existe
    /// </summary>
    /// <param name="id">ID do post</param>
    /// <returns>True se existir, False caso contrário</returns>
    Task<bool> ExistsAsync(Guid id);
    
    /// <summary>
    /// Verifica se um slug já está sendo usado
    /// Útil para evitar slugs duplicados ao criar/editar posts
    /// </summary>
    /// <param name="slug">Slug a verificar</param>
    /// <param name="excludeId">ID do post a excluir da verificação (para updates)</param>
    /// <returns>True se o slug já existe, False caso contrário</returns>
    Task<bool> SlugExistsAsync(string slug, Guid? excludeId = null);
    
    // ==========================================
    // PERSISTÊNCIA
    // ==========================================
    
    /// <summary>
    /// Salva todas as mudanças no banco de dados
    /// Usado após AddAsync, UpdateAsync, DeleteAsync
    /// </summary>
    Task<int> SaveChangesAsync();
}