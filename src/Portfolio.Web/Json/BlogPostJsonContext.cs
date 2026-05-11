using System.Text.Json.Serialization;
using Portfolio.Web.DTOs.BlogPosts;

namespace Portfolio.Web.Json;

[JsonSerializable(typeof(List<BlogPostCardDto>))]
[JsonSerializable(typeof(BlogPostCardDto))]
[JsonSerializable(typeof(BlogPostDto))]
[JsonSourceGenerationOptions(
    PropertyNameCaseInsensitive = true,
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
)]
public partial class BlogPostJsonContext : JsonSerializerContext
{
}