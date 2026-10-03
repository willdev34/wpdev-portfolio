// ====================================
// Título: BulkDeleteBlogPostsDto.cs
// Descrição: DTOs da exclusão em massa de posts (espelham os da Application)
// ====================================

namespace Portfolio.Web.DTOs.BlogPosts;

public class BulkDeleteBlogPostsDto
{
    public List<Guid> Ids { get; set; } = new();
}

public class BulkDeleteBlogPostsResultDto
{
    public int DeletedCount { get; set; }
}
