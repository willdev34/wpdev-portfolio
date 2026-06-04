// ====================================
// Título: ProjectService.cs
// Descrição: Serviço para consumir a API de projetos
// ====================================

using System.Net;
using System.Text.Json;
using Portfolio.Web.DTOs.Projects;
using Portfolio.Web.Json;

namespace Portfolio.Web.Services;

public class ProjectService
{
    private readonly HttpClient _httpClient;

    public ProjectService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    // ====================================
    // GET ALL: Busca todos os projetos
    // ====================================
    public async Task<List<ProjectCardDto>> GetAllAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("api/projects");

            // Se a API respondeu com erro, loga e retorna lista vazia
            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"[ProjectService] GetAllAsync retornou {(int)response.StatusCode} {response.StatusCode}");
                return new List<ProjectCardDto>();
            }

            var json = await response.Content.ReadAsStringAsync();
            var projects = JsonSerializer.Deserialize(json, ProjectJsonContext.Default.ListProjectCardDto);
            return projects ?? new List<ProjectCardDto>();
        }
        catch (HttpRequestException ex)
        {
            // Falha de rede, CORS, DNS, API fora do ar
            Console.WriteLine($"[ProjectService] Erro de rede em GetAllAsync: {ex.Message}");
            return new List<ProjectCardDto>();
        }
        catch (JsonException ex)
        {
            // API retornou um JSON malformado ou com formato inesperado
            Console.WriteLine($"[ProjectService] Erro de deserialização em GetAllAsync: {ex.Message}");
            return new List<ProjectCardDto>();
        }
        catch (Exception ex)
        {
            // Qualquer outra coisa que não previmos
            Console.WriteLine($"[ProjectService] Erro inesperado em GetAllAsync: {ex.Message}");
            return new List<ProjectCardDto>();
        }
    }

    // ====================================
    // GET BY ID: Busca projeto específico
    // ====================================
    public async Task<ProjectDto?> GetByIdAsync(Guid id)
    {
        try
        {
            var response = await _httpClient.GetAsync($"api/projects/{id}");

            // 404 é caso de uso normal, não é erro de fato
            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return null;
            }

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"[ProjectService] GetByIdAsync({id}) retornou {(int)response.StatusCode} {response.StatusCode}");
                return null;
            }

            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize(json, ProjectJsonContext.Default.ProjectDto);
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"[ProjectService] Erro de rede em GetByIdAsync({id}): {ex.Message}");
            return null;
        }
        catch (JsonException ex)
        {
            Console.WriteLine($"[ProjectService] Erro de deserialização em GetByIdAsync({id}): {ex.Message}");
            return null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ProjectService] Erro inesperado em GetByIdAsync({id}): {ex.Message}");
            return null;
        }
    }

    // ====================================
    // GET FEATURED: Filtra projetos em destaque
    // ====================================
    // OBS: Reutiliza GetAllAsync que já tem tratamento de erros completo.
    // Como GetAllAsync nunca lança exceção, não precisamos de try/catch aqui.
    public async Task<List<ProjectCardDto>> GetFeaturedAsync()
    {
        var allProjects = await GetAllAsync();
        return allProjects.Where(p => p.IsFeatured).ToList();
    }
}