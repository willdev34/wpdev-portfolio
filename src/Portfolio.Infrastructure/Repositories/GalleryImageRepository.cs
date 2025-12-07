// ====================================
// Título: GalleryImageRepository.cs
// Descrição: Implementação do Repository usando EF Core para GalleryImage
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
/// Implementação do Repository para GalleryImage usando Entity Framework Core
/// Responsável por todas as operações de acesso a dados da entidade GalleryImage
/// </summary>
public class GalleryImageRepository : IGalleryImageRepository
{
    private readonly PortfolioDbContext _context;

    public GalleryImageRepository(PortfolioDbContext context)
    {
        _context = context;
    }

    // ==========================================
    // CONSULTAS (QUERIES)
    // ==========================================

    /// <summary>
    /// Busca TODAS as imagens da galeria
    /// Ordenadas por Order (crescente)
    /// </summary>
    public async Task<IEnumerable<GalleryImage>> GetAllAsync()
    {
        return await _context.GalleryImages
            .OrderBy(i => i.Order)
            .ToListAsync();
    }

    /// <summary>
    /// Busca uma imagem específica por ID
    /// </summary>
    public async Task<GalleryImage?> GetByIdAsync(Guid id)
    {
        return await _context.GalleryImages
            .FirstOrDefaultAsync(i => i.Id == id);
    }

    /// <summary>
    /// Busca apenas imagens VISÍVEIS (IsVisible = true)
    /// Ordenadas por Order (crescente)
    /// Usado para exibir a galeria no site público
    /// </summary>
    public async Task<IEnumerable<GalleryImage>> GetVisibleAsync()
    {
        return await _context.GalleryImages
            .Where(i => i.IsVisible)
            .OrderBy(i => i.Order)
            .ToListAsync();
    }

    /// <summary>
    /// Busca imagens que contêm uma tag específica
    /// Usa EF.Functions.ILike para busca case-insensitive
    /// </summary>
    public async Task<IEnumerable<GalleryImage>> GetByTagAsync(string tag)
    {
        return await _context.GalleryImages
            .Where(i => i.IsVisible && i.Tags.Any(t => EF.Functions.ILike(t, tag)))
            .OrderBy(i => i.Order)
            .ToListAsync();
    }

    // ==========================================
    // COMANDOS (MUTATIONS)
    // ==========================================

    /// <summary>
    /// Adiciona uma nova imagem ao contexto
    /// IMPORTANTE: Não salva automaticamente, precisa chamar SaveChangesAsync()
    /// </summary>
    public async Task<GalleryImage> AddAsync(GalleryImage galleryImage)
    {
        // Gera um novo ID
        galleryImage.Id = Guid.NewGuid();
        
        // Seta a data de criação
        galleryImage.CreatedAt = DateTime.UtcNow;
        
        // Garante que está visível por padrão
        if (!galleryImage.IsVisible)
        {
            galleryImage.IsVisible = true;
        }

        await _context.GalleryImages.AddAsync(galleryImage);
        
        return galleryImage;
    }

    /// <summary>
    /// Atualiza uma imagem existente
    /// IMPORTANTE: Não salva automaticamente, precisa chamar SaveChangesAsync()
    /// </summary>
    public async Task UpdateAsync(GalleryImage galleryImage)
    {
        // Seta a data de atualização
        galleryImage.UpdatedAt = DateTime.UtcNow;

        _context.GalleryImages.Update(galleryImage);
        
        await Task.CompletedTask; // Para manter assinatura async
    }

    /// <summary>
    /// Soft delete - marca a imagem como invisível ao invés de deletar fisicamente
    /// </summary>
    public async Task DeleteAsync(Guid id)
    {
        var galleryImage = await _context.GalleryImages.FindAsync(id);
        
        if (galleryImage != null)
        {
            galleryImage.IsVisible = false;
            galleryImage.UpdatedAt = DateTime.UtcNow;
            _context.GalleryImages.Update(galleryImage);
        }
    }

    /// <summary>
    /// Verifica se uma imagem existe no banco
    /// </summary>
    public async Task<bool> ExistsAsync(Guid id)
    {
        return await _context.GalleryImages.AnyAsync(i => i.Id == id);
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