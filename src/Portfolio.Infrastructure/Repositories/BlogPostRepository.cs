// ====================================
// Título: BlogPostRepository.cs
// Descrição: Implementação do Repository usando EF Core para BlogPost
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
/// Implementação do Repository para BlogPost usando Entity Framework Core
/// Responsável por todas as operações de acesso a dados da entidade BlogPost
/// </summary>
public class BlogPostRepository : IBlogPostRepository
{
    private readonly PortfolioDbContext _context;

    public BlogPostRepository(PortfolioDbContext context)
    {
        _context = context;
    }

    // ==========================================
    // CONSULTAS (QUERIES)
    // ==========================================

    /// <summary>
    /// Busca TODOS os posts (publicados e rascunhos)
    /// Ordenados por data de criação (mais recentes primeiro)
    /// </summary>
    public async Task<IEnumerable<BlogPost>> GetAllAsync()
    {
        return await _context.BlogPosts
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();
    }

    /// <summary>
    /// Busca um post específico por ID
    /// </summary>
    public async Task<BlogPost?> GetByIdAsync(Guid id)
    {
        return await _context.BlogPosts
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    /// <summary>
    /// Busca um post por Slug (URL amigável)
    /// Exemplo: /blog/como-aprender-dotnet
    /// </summary>
    public async Task<BlogPost?> GetBySlugAsync(string slug)
    {
        return await _context.BlogPosts
            .FirstOrDefaultAsync(p => p.Slug == slug);
    }

    /// <summary>
    /// Busca apenas posts PUBLICADOS
    /// Ordenados por data de publicação (mais recentes primeiro)
    /// </summary>
    public async Task<IEnumerable<BlogPost>> GetPublishedAsync()
    {
        return await _context.BlogPosts
            .Where(p => p.IsPublished && p.PublishedAt != null)
            .OrderByDescending(p => p.PublishedAt)
            .ToListAsync();
    }

    /// <summary>
    /// Busca posts em destaque
    /// Ordenados por data de publicação (mais recentes primeiro)
    /// </summary>
    public async Task<IEnumerable<BlogPost>> GetFeaturedAsync()
    {
        return await _context.BlogPosts
            .Where(p => p.IsPublished && p.IsFeatured)
            .OrderByDescending(p => p.PublishedAt)
            .ToListAsync();
    }

    /// <summary>
    /// Busca posts que contêm uma tag específica
    /// Usa EF.Functions.ILike para busca case-insensitive
    /// </summary>
    public async Task<IEnumerable<BlogPost>> GetByTagAsync(string tag)
    {
        return await _context.BlogPosts
            .Where(p => p.IsPublished && p.Tags.Any(t => EF.Functions.ILike(t, tag)))
            .OrderByDescending(p => p.PublishedAt)
            .ToListAsync();
    }

    // ==========================================
    // COMANDOS (MUTATIONS)
    // ==========================================

    /// <summary>
    /// Adiciona um novo post ao contexto
    /// IMPORTANTE: Não salva automaticamente, precisa chamar SaveChangesAsync()
    /// </summary>
    public async Task<BlogPost> AddAsync(BlogPost blogPost)
    {
        // Gera um novo ID
        blogPost.Id = Guid.NewGuid();
        
        // Seta a data de criação
        blogPost.CreatedAt = DateTime.UtcNow;
        
        // Se foi marcado como publicado, seta a data de publicação
        if (blogPost.IsPublished && blogPost.PublishedAt == null)
        {
            blogPost.PublishedAt = DateTime.UtcNow;
        }

        await _context.BlogPosts.AddAsync(blogPost);
        
        return blogPost;
    }

    /// <summary>
    /// Atualiza um post existente
    /// IMPORTANTE: Não salva automaticamente, precisa chamar SaveChangesAsync()
    /// </summary>
    public async Task UpdateAsync(BlogPost blogPost)
    {
        // Seta a data de atualização
        blogPost.UpdatedAt = DateTime.UtcNow;
        
        // Se mudou de rascunho para publicado, seta a data de publicação
        var existingPost = await _context.BlogPosts
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == blogPost.Id);
        
        if (existingPost != null && !existingPost.IsPublished && blogPost.IsPublished)
        {
            blogPost.PublishedAt = DateTime.UtcNow;
        }

        _context.BlogPosts.Update(blogPost);
        
        await Task.CompletedTask; // Para manter assinatura async
    }

    /// <summary>
    /// Deleta um post FISICAMENTE do banco
    /// ATENÇÃO: BlogPost NÃO usa soft delete
    /// </summary>
    public async Task DeleteAsync(Guid id)
    {
        var blogPost = await _context.BlogPosts.FindAsync(id);
        
        if (blogPost != null)
        {
            _context.BlogPosts.Remove(blogPost);
        }
    }

    /// <summary>
    /// Incrementa o contador de visualizações de um post
    /// Usado quando alguém acessa a página do post
    /// </summary>
    public async Task IncrementViewCountAsync(Guid id)
    {
        var blogPost = await _context.BlogPosts.FindAsync(id);
        
        if (blogPost != null)
        {
            blogPost.ViewCount++;
            _context.BlogPosts.Update(blogPost);
        }
    }

    /// <summary>
    /// Verifica se um post existe no banco
    /// </summary>
    public async Task<bool> ExistsAsync(Guid id)
    {
        return await _context.BlogPosts.AnyAsync(p => p.Id == id);
    }

    /// <summary>
    /// Verifica se um slug já está sendo usado
    /// Útil para evitar URLs duplicadas
    /// </summary>
    /// <param name="slug">Slug a verificar</param>
    /// <param name="excludeId">ID do post a excluir da verificação (usado em updates)</param>
    public async Task<bool> SlugExistsAsync(string slug, Guid? excludeId = null)
    {
        if (excludeId.HasValue)
        {
            return await _context.BlogPosts
                .AnyAsync(p => p.Slug == slug && p.Id != excludeId.Value);
        }
        
        return await _context.BlogPosts.AnyAsync(p => p.Slug == slug);
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