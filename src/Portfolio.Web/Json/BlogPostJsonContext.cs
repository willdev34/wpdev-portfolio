using System.Text.Json.Serialization;
using Portfolio.Web.DTOs.BlogPosts;

namespace Portfolio.Web.Json;

[JsonSerializable(typeof(List<BlogPostCardDto>))]
[JsonSerializable(typeof(BlogPostCardDto))]
[JsonSerializable(typeof(List<BlogPostAdminCardDto>))]
[JsonSerializable(typeof(BlogPostAdminCardDto))]
[JsonSerializable(typeof(BlogPostDto))]
[JsonSerializable(typeof(CreateBlogPostDto))]
[JsonSerializable(typeof(UpdateBlogPostDto))]
[JsonSerializable(typeof(BulkDeleteBlogPostsDto))]
[JsonSerializable(typeof(BulkDeleteBlogPostsResultDto))]
[JsonSerializable(typeof(List<string>))]
[JsonSourceGenerationOptions(
    PropertyNameCaseInsensitive = true,
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
)]
public partial class BlogPostJsonContext : JsonSerializerContext
{
}