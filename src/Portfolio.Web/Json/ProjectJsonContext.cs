// ====================================
// Título: ProjectJsonContext.cs
// Descrição: Source generator para serialização dos DTOs de projetos
// ====================================

using System.Text.Json.Serialization;
using Portfolio.Web.DTOs.Projects;

namespace Portfolio.Web.Json;

[JsonSerializable(typeof(List<ProjectCardDto>))]
[JsonSerializable(typeof(ProjectCardDto))]
[JsonSerializable(typeof(ProjectDto))]
[JsonSerializable(typeof(CreateProjectDto))]
[JsonSerializable(typeof(UpdateProjectDto))]
[JsonSerializable(typeof(List<string>))]
[JsonSourceGenerationOptions(
    PropertyNameCaseInsensitive = true,
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
)]
public partial class ProjectJsonContext : JsonSerializerContext
{
}