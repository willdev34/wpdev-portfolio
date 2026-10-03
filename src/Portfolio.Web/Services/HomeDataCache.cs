using Portfolio.Web.DTOs.BlogPosts;
using Portfolio.Web.DTOs.Projects;

namespace Portfolio.Web.Services;

/// <summary>
/// Cache em memória dos dados da home.
/// Preenchido no boot do app, consumido pelo Home.razor.
/// Projetos e últimos posts são buscados em paralelo e de forma independente:
/// a falha de um não afeta o outro.
/// </summary>
public class HomeDataCache
{
    private const int LatestPostsCount = 3;

    private TaskCompletionSource<List<ProjectCardDto>> _featuredTcs = new();
    private TaskCompletionSource<List<BlogPostCardDto>> _latestPostsTcs = new();

    /// <summary>
    /// Inicia o fetch dos dados featured e dos últimos posts.
    /// Chamado no Program.cs, antes do app renderizar.
    /// </summary>
    public void StartPreload(ProjectService projectService, BlogPostService blogPostService)
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

        _ = Task.Run(async () =>
        {
            try
            {
                var posts = await blogPostService.GetLatestAsync(LatestPostsCount);
                _latestPostsTcs.TrySetResult(posts);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[HomeDataCache] Preload dos posts falhou: {ex.Message}");
                _latestPostsTcs.TrySetResult(new List<BlogPostCardDto>());
            }
        });
    }

    /// <summary>
    /// Aguarda o resultado do preload. Se já chegou, retorna direto.
    /// </summary>
    public Task<List<ProjectCardDto>> GetFeaturedAsync()
        => _featuredTcs.Task;

    /// <summary>
    /// Resultado do preload dos últimos posts. Nunca falha: em erro, vem lista vazia.
    /// A Home não espera por isso para esconder a splash.
    /// </summary>
    public Task<List<BlogPostCardDto>> GetLatestPostsAsync()
        => _latestPostsTcs.Task;

    /// <summary>
    /// Invalida o cache (após CRUD no admin, por exemplo).
    /// </summary>
    public void Invalidate()
    {
        _featuredTcs = new TaskCompletionSource<List<ProjectCardDto>>();
        _latestPostsTcs = new TaskCompletionSource<List<BlogPostCardDto>>();
    }
}
