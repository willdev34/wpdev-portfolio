using Portfolio.Web.DTOs.Projects;

namespace Portfolio.Web.Services;

/// <summary>
/// Cache em memória dos dados da home.
/// Preenchido no boot do app, consumido pelo Home.razor.
/// </summary>
public class HomeDataCache
{
    private TaskCompletionSource<List<ProjectCardDto>> _featuredTcs = new();

    /// <summary>
    /// Inicia o fetch dos dados featured.
    /// Chamado no Program.cs, antes do app renderizar.
    /// </summary>
    public void StartPreload(ProjectService projectService)
    {
        _ = Task.Run(async () =>
        {
            try
            {
                var projects = await projectService.GetFeaturedAsync();
                _featuredTcs.TrySetResult(projects);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[HomeDataCache] Preload falhou: {ex.Message}");
                _featuredTcs.TrySetResult(new List<ProjectCardDto>());
            }
        });
    }

    /// <summary>
    /// Aguarda o resultado do preload. Se já chegou, retorna direto.
    /// </summary>
    public Task<List<ProjectCardDto>> GetFeaturedAsync()
        => _featuredTcs.Task;

    /// <summary>
    /// Invalida o cache (após CRUD no admin, por exemplo).
    /// </summary>
    public void Invalidate()
    {
        _featuredTcs = new TaskCompletionSource<List<ProjectCardDto>>();
    }
}