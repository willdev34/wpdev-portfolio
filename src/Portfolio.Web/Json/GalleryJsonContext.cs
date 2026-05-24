using System.Text.Json.Serialization;
using Portfolio.Web.DTOs.Gallery;

namespace Portfolio.Web.Json;

[JsonSerializable(typeof(List<GalleryImageDto>))]
[JsonSerializable(typeof(GalleryImageDto))]
[JsonSourceGenerationOptions(
    PropertyNameCaseInsensitive = true,
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
)]
public partial class GalleryJsonContext : JsonSerializerContext
{
}