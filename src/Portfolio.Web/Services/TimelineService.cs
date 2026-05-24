// ====================================
// Título: TimelineService.cs
// Descrição: Serviço para consumir endpoints de Timeline da API
// ====================================

using Portfolio.Web.DTOs.Timeline;
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

    // Buscar todos os eventos da timeline
    public async Task<List<TimelineEventDto>> GetAllAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("api/timelineevents");

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"[TimelineService] GetAllAsync falhou. Status: {(int)response.StatusCode} {response.StatusCode}");
                return new List<TimelineEventDto>();
            }

            var json = await response.Content.ReadAsStringAsync();
            var events = JsonSerializer.Deserialize<List<TimelineEventDto>>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return events ?? new List<TimelineEventDto>();
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"[TimelineService] GetAllAsync - Erro de rede: {ex.Message}");
            return new List<TimelineEventDto>();
        }
        catch (JsonException ex)
        {
            Console.WriteLine($"[TimelineService] GetAllAsync - Erro ao desserializar JSON: {ex.Message}");
            return new List<TimelineEventDto>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[TimelineService] GetAllAsync - Erro inesperado: {ex.Message}");
            return new List<TimelineEventDto>();
        }
    }

    // Buscar evento por ID
    public async Task<TimelineEventDto?> GetByIdAsync(Guid id)
    {
        try
        {
            var response = await _httpClient.GetAsync($"api/timelineevents/{id}");

            // 404 é caso esperado, não é bug
            if (response.StatusCode == HttpStatusCode.NotFound)
                return null;

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"[TimelineService] GetByIdAsync({id}) falhou. Status: {(int)response.StatusCode} {response.StatusCode}");
                return null;
            }

            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<TimelineEventDto>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"[TimelineService] GetByIdAsync({id}) - Erro de rede: {ex.Message}");
            return null;
        }
        catch (JsonException ex)
        {
            Console.WriteLine($"[TimelineService] GetByIdAsync({id}) - Erro ao desserializar JSON: {ex.Message}");
            return null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[TimelineService] GetByIdAsync({id}) - Erro inesperado: {ex.Message}");
            return null;
        }
    }
}