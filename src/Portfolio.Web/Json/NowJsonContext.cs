using System.Text.Json.Serialization;
using Portfolio.Web.DTOs.Now;

namespace Portfolio.Web.Json;

[JsonSerializable(typeof(List<NowSectionDto>))]
[JsonSerializable(typeof(NowSectionDto))]
[JsonSerializable(typeof(CreateNowSectionDto))]
[JsonSerializable(typeof(UpdateNowSectionDto))]
[JsonSerializable(typeof(List<ProjectLink>))]
[JsonSerializable(typeof(ProjectLink))]
[JsonSourceGenerationOptions(
    PropertyNameCaseInsensitive = true,
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
)]
public partial class NowJsonContext : JsonSerializerContext
{
}