// ====================================
// Título: IGalleryImageRepository.cs
// Descrição: Interface do Repository para GalleryImage
// Autor: Will
// Empresa: WpDev
// Data: 06/12/2024
// ====================================

using Portfolio.Domain.Entities;

namespace Portfolio.Application.Interfaces;

/// <summary>
/// Interface do Repository para a entidade GalleryImage
/// Define o CONTRATO de quais operações de acesso a dados devem existir
/// A implementação fica na camada Infrastructure
/// </summary>
public interface IGalleryImageRepository
{
    // ==========================================
    // CONSULTAS (QUERIES)
    // ==========================================
    
    /// <summary>
    /// Busca TODAS as imagens da galeria
    /// Ordenadas por Order (crescente)
    /// </summary>
    /// <returns>Lista de todas as imagens</returns>
    Task<IEnumerable<GalleryImage>> GetAllAsync();
    
    /// <summary>
    /// Busca uma imagem por ID
    /// </summary>
    /// <param name="id">ID da imagem</param>
    /// <returns>Imagem encontrada ou null</returns>
    Task<GalleryImage?> GetByIdAsync(Guid id);
    
    /// <summary>
    /// Busca apenas imagens VISÍVEIS (IsVisible = true)
    /// Ordenadas por Order (crescente)
    /// Usado para exibir a galeria no site público
    /// </summary>
    /// <returns>Lista de imagens visíveis</returns>
    Task<IEnumerable<GalleryImage>> GetVisibleAsync();
    
    /// <summary>
    /// Busca imagens por tag
    /// </summary>
    /// <param name="tag">Nome da tag (ex: "React", "Design")</param>
    /// <returns>Lista de imagens que contêm a tag</returns>
    Task<IEnumerable<GalleryImage>> GetByTagAsync(string tag);
    
    // ==========================================
    // COMANDOS (MUTATIONS)
    // ==========================================
    
    /// <summary>
    /// Adiciona uma nova imagem ao banco
    /// </summary>
    /// <param name="galleryImage">Imagem a ser adicionada</param>
    /// <returns>Imagem adicionada com ID gerado</returns>
    Task<GalleryImage> AddAsync(GalleryImage galleryImage);
    
    /// <summary>
    /// Atualiza uma imagem existente
    /// </summary>
    /// <param name="galleryImage">Imagem com dados atualizados</param>
    Task UpdateAsync(GalleryImage galleryImage);
    
    /// <summary>
    /// Deleta uma imagem (soft delete - apenas marca como invisível)
    /// </summary>
    /// <param name="id">ID da imagem a deletar</param>
    Task DeleteAsync(Guid id);
    
    /// <summary>
    /// Verifica se uma imagem existe
    /// </summary>
    /// <param name="id">ID da imagem</param>
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