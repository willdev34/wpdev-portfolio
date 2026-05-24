// ====================================
// Título: TimelineService.cs
// Descrição: Serviço para consumir endpoints de Timeline da API
// ====================================
using Portfolio.Web.DTOs.Timeline;
using Portfolio.Web.Json;
using System.Net;
using System.Text.Json;

namespace Portfolio.Web.Services;

public class TimelineService
{
    private readonly HttpClient _httpClient;

    public TimelineService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<TimelineEventDto>> GetAllAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("api/timelineevents");
            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"[TimelineService] GetAllAsync falhou. Status: {(int)response.StatusCode}");
                return new List<TimelineEventDto>();
            }
            var json = await response.Content.ReadAsStringAsync();
            var events = JsonSerializer.Deserialize(json, TimelineJsonContext.Default.ListTimelineEventDto);
            return events ?? new List<TimelineEventDto>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[TimelineService] GetAllAsync - Erro inesperado: {ex.Message}");
            return new List<TimelineEventDto>();
        }
    }

    public async Task<TimelineEventDto?> GetByIdAsync(Guid id)
    {
        try
        {
            var response = await _httpClient.GetAsync($"api/timelineevents/{id}");
            if (response.StatusCode == HttpStatusCode.NotFound)
                return null;
            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"[TimelineService] GetByIdAsync({id}) falhou. Status: {(int)response.StatusCode}");
                return null;
            }
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize(json, TimelineJsonContext.Default.TimelineEventDto);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[TimelineService] GetByIdAsync({id}) - Erro inesperado: {ex.Message}");
            return null;
        }
    }
}