// ====================================
// Título: TimelineService.cs
// Descrição: Serviço para consumir endpoints de Timeline da API
// Autor: Will
// Empresa: WpDev
// Data: 09/12/2024
// ====================================

using Portfolio.Web.DTOs.Timeline;
using System.Net.Http.Json;

namespace Portfolio.Web.Services;

public class TimelineService
{
    private readonly HttpClient _httpClient;

    public TimelineService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    // Buscar todos os eventos
    public async Task<List<TimelineEventDto>> GetAllAsync()
    {
        try
        {
            var events = await _httpClient.GetFromJsonAsync<List<TimelineEventDto>>("api/timeline");
            return events ?? new List<TimelineEventDto>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao buscar eventos: {ex.Message}");
            return new List<TimelineEventDto>();
        }
    }

    // Buscar evento por ID
    public async Task<TimelineEventDto?> GetByIdAsync(Guid id)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<TimelineEventDto>($"api/timeline/{id}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao buscar evento {id}: {ex.Message}");
            return null;
        }
    }
}