using System.Text.Json.Serialization;
using Portfolio.Web.DTOs.Projects;

namespace Portfolio.Web.Json;

[JsonSerializable(typeof(List<ProjectCardDto>))]
[JsonSerializable(typeof(ProjectCardDto))]
[JsonSerializable(typeof(ProjectDto))]
[JsonSourceGenerationOptions(
    PropertyNameCaseInsensitive = true,
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
)]
public partial class ProjectJsonContext : JsonSerializerContext
{
}