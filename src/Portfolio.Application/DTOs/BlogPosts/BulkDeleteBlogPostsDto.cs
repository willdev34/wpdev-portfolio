// ====================================
// Título: BulkDeleteBlogPostsDto.cs
// Descrição: DTOs de entrada e saída da exclusão em massa de posts
// ====================================

namespace Portfolio.Application.DTOs.BlogPosts;

/// <summary>
/// Corpo do POST /api/blogposts/bulk-delete
/// </summary>
public class BulkDeleteBlogPostsDto
{
    /// <summary>
    /// IDs dos posts a excluir (1 a 100, sem Guid.Empty; duplicados são ignorados)
    /// </summary>
    public List<Guid> Ids { get; set; } = new();
}

/// <summary>
/// Resposta da exclusão em massa
/// </summary>
public class BulkDeleteBlogPostsResultDto
{
    /// <summary>
    /// Quantos posts foram de fato excluídos (ids inexistentes não contam)
    /// </summary>
    public int DeletedCount { get; set; }
}
