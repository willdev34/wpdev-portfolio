// Título: ProjectFormMapper.cs
// Descrição: Conversão entre o projeto carregado da API, o formulário do admin e o DTO de atualização

using Portfolio.Web.DTOs.Projects;

namespace Portfolio.Web.Helpers;

/// <summary>Formulário carregado e, se o status vindo da API não for reconhecido, o texto recebido (senão null).</summary>
public sealed record ProjectFormLoadResult(CreateProjectDto Form, string? UnknownStatus);

/// <summary>
/// Carregar, editar e salvar um projeto sem mexer em nada nunca pode alterar nada. Por isso a ida (ToForm) copia todos
/// os campos editáveis e a volta (ToUpdate) parte do projeto original para os campos que o formulário não edita
/// (Id e IsActive), em vez de inventar valores fixos.
/// </summary>
public static class ProjectFormMapper
{
    public static ProjectFormLoadResult ToForm(ProjectDto project)
    {
        var recognized = ProjectStatusHelper.TryParse(project.Status, out var status);

        var form = new CreateProjectDto
        {
            Title = project.Title,
            Description = project.Description,
            ShortDescription = project.ShortDescription,
            ImageUrl = project.ImageUrl,
            DemoUrl = project.DemoUrl,
            RepositoryUrl = project.RepositoryUrl,
            Technologies = new List<string>(project.Technologies),
            Year = project.Year,
            IsFeatured = project.IsFeatured,
            Status = recognized ? status : ProjectStatusHelper.Unknown
        };

        var unknown = recognized ? null : (string.IsNullOrWhiteSpace(project.Status) ? "(vazio)" : project.Status);

        return new ProjectFormLoadResult(form, unknown);
    }

    public static UpdateProjectDto ToUpdate(ProjectDto original, CreateProjectDto form) => new()
    {
        Id = original.Id,
        Title = form.Title,
        Description = form.Description,
        ShortDescription = form.ShortDescription,
        ImageUrl = form.ImageUrl,
        DemoUrl = form.DemoUrl,
        RepositoryUrl = form.RepositoryUrl,
        Technologies = new List<string>(form.Technologies),
        Year = form.Year,
        IsFeatured = form.IsFeatured,
        Status = form.Status,
        IsActive = original.IsActive
    };
}
