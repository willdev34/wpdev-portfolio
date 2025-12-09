// ====================================
// Título: ProjectService.cs
// Descrição: Serviço para consumir a API de projetos
// Autor: Will
// Empresa: WpDev
// Data: 07/12/2024
// ====================================

using System.Net.Http.Json;
using Portfolio.Web.DTOs.Projects;

namespace Portfolio.Web.Services;

/// <summary>
/// Serviço responsável por consumir a API de projetos
/// Faz as requisições HTTP e retorna os DTOs
/// </summary>
public class ProjectService
{
    private readonly HttpClient _httpClient;

    // ====================================
    // CONSTRUTOR - Injeção de Dependência
    // ====================================
    public ProjectService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    // ====================================
    // GET ALL PROJECTS
    // ====================================
    /// <summary>
    /// Busca todos os projetos ativos
    /// Endpoint: GET /api/projects
    /// </summary>
    public async Task<List<ProjectCardDto>> GetAllAsync()
    {
        try
        {
            var projects = await _httpClient.GetFromJsonAsync<List<ProjectCardDto>>("api/projects");
            return projects ?? new List<ProjectCardDto>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao buscar projetos: {ex.Message}");
            return new List<ProjectCardDto>();
        }
    }

    // ====================================
    // GET PROJECT BY ID
    // ====================================
    /// <summary>
    /// Busca um projeto específico por ID
    /// Endpoint: GET /api/projects/{id}
    /// </summary>
    public async Task<ProjectDto?> GetByIdAsync(Guid id)
    {
        try
        {
            var project = await _httpClient.GetFromJsonAsync<ProjectDto>($"api/projects/{id}");
            return project;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao buscar projeto {id}: {ex.Message}");
            return null;
        }
    }

    // ====================================
    // GET FEATURED PROJECTS
    // ====================================
    /// <summary>
    /// Busca apenas projetos em destaque
    /// Para usar na home page
    /// </summary>
    public async Task<List<ProjectCardDto>> GetFeaturedAsync()
    {
        try
        {
            var allProjects = await GetAllAsync();
            return allProjects.Where(p => p.IsFeatured).ToList();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao buscar projetos em destaque: {ex.Message}");
            return new List<ProjectCardDto>();
        }
    }
}