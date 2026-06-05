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
    // GET ALL
    // ====================================
    public async Task<List<ProjectCardDto>> GetAllAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("api/projects");

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"[ProjectService] GetAllAsync retornou {(int)response.StatusCode}");
                return new List<ProjectCardDto>();
            }

            var json = await response.Content.ReadAsStringAsync();
            var projects = JsonSerializer.Deserialize(json, ProjectJsonContext.Default.ListProjectCardDto);
            return projects ?? new List<ProjectCardDto>();
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"[ProjectService] Erro de rede em GetAllAsync: {ex.Message}");
            return new List<ProjectCardDto>();
        }
        catch (JsonException ex)
        {
            Console.WriteLine($"[ProjectService] Erro de deserialização em GetAllAsync: {ex.Message}");
            return new List<ProjectCardDto>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ProjectService] Erro inesperado em GetAllAsync: {ex.Message}");
            return new List<ProjectCardDto>();
        }
    }

    // ====================================
    // GET BY ID
    // ====================================
    public async Task<ProjectDto?> GetByIdAsync(Guid id)
    {
        try
        {
            var response = await _httpClient.GetAsync($"api/projects/{id}");

            if (response.StatusCode == HttpStatusCode.NotFound)
                return null;

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"[ProjectService] GetByIdAsync({id}) retornou {(int)response.StatusCode}");
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
    // GET FEATURED
    // ====================================
    public async Task<List<ProjectCardDto>> GetFeaturedAsync()
    {
        var allProjects = await GetAllAsync();
        return allProjects.Where(p => p.IsFeatured).ToList();
    }

    // ====================================
    // CREATE (Admin)
    // ====================================
    public async Task<(bool Success, string? Error)> CreateAsync(CreateProjectDto dto)
    {
        try
        {
            var json = JsonSerializer.Serialize(dto, ProjectJsonContext.Default.CreateProjectDto);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("api/projects", content);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"[ProjectService] CreateAsync retornou {(int)response.StatusCode}: {error}");
                return (false, $"Erro {(int)response.StatusCode}: {error}");
            }

            return (true, null);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ProjectService] Erro em CreateAsync: {ex.Message}");
            return (false, ex.Message);
        }
    }

    // ====================================
    // UPDATE (Admin)
    // ====================================
    public async Task<(bool Success, string? Error)> UpdateAsync(UpdateProjectDto dto)
    {
        try
        {
            var json = JsonSerializer.Serialize(dto, ProjectJsonContext.Default.UpdateProjectDto);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync($"api/projects/{dto.Id}", content);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"[ProjectService] UpdateAsync({dto.Id}) retornou {(int)response.StatusCode}: {error}");
                return (false, $"Erro {(int)response.StatusCode}: {error}");
            }

            return (true, null);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ProjectService] Erro em UpdateAsync: {ex.Message}");
            return (false, ex.Message);
        }
    }

    // ====================================
    // DELETE (Admin)
    // ====================================
    public async Task<(bool Success, string? Error)> DeleteAsync(Guid id)
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"api/projects/{id}");

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"[ProjectService] DeleteAsync({id}) retornou {(int)response.StatusCode}: {error}");
                return (false, $"Erro {(int)response.StatusCode}: {error}");
            }

            return (true, null);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ProjectService] Erro em DeleteAsync: {ex.Message}");
            return (false, ex.Message);
        }
    }
}