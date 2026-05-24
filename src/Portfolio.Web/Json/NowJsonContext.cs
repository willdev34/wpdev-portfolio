using System.Text.Json.Serialization;
using Portfolio.Web.DTOs.Now;

namespace Portfolio.Web.Json;

[JsonSerializable(typeof(List<NowSectionDto>))]
[JsonSerializable(typeof(NowSectionDto))]
[JsonSourceGenerationOptions(
    PropertyNameCaseInsensitive = true,
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
)]
public partial class NowJsonContext : JsonSerializerContext
{
}