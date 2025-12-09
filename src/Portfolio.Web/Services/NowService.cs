// ====================================
// Título: NowService.cs
// Descrição: Serviço para consumir endpoints de Now da API
// Autor: Will
// Empresa: WpDev
// Data: 09/12/2024
// ====================================

using Portfolio.Web.DTOs.Now;
using System.Net.Http.Json;

namespace Portfolio.Web.Services;

public class NowService
{
    private readonly HttpClient _httpClient;

    public NowService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    // Buscar a seção Now atual
    public async Task<NowSectionDto?> GetCurrentAsync()
    {
        try
        {
            var sections = await _httpClient.GetFromJsonAsync<List<NowSectionDto>>("api/now");
            return sections?.OrderByDescending(n => n.LastUpdated).FirstOrDefault();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao buscar seção Now: {ex.Message}");
            return null;
        }
    }

    // Buscar por ID
    public async Task<NowSectionDto?> GetByIdAsync(Guid id)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<NowSectionDto>($"api/now/{id}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao buscar seção {id}: {ex.Message}");
            return null;
        }
    }
}