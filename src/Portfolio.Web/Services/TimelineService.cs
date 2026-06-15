// ====================================
// Título: TimelineService.cs
// Descrição: Serviço para consumir endpoints de Timeline da API
// ====================================
using Portfolio.Web.DTOs.Timeline;
using Portfolio.Web.Json;
using System.Net;
using System.Text;
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
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"[TimelineService] GetAllAsync - Erro de rede: {ex.Message}");
            return new List<TimelineEventDto>();
        }
        catch (JsonException ex)
        {
            Console.WriteLine($"[TimelineService] GetAllAsync - Erro de JSON: {ex.Message}");
            return new List<TimelineEventDto>();
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
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"[TimelineService] GetByIdAsync({id}) - Erro de rede: {ex.Message}");
            return null;
        }
        catch (JsonException ex)
        {
            Console.WriteLine($"[TimelineService] GetByIdAsync({id}) - Erro de JSON: {ex.Message}");
            return null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[TimelineService] GetByIdAsync({id}) - Erro inesperado: {ex.Message}");
            return null;
        }
    }

    public async Task<(bool Success, string? Error)> CreateAsync(CreateTimelineEventDto dto)
    {
        try
        {
            var json = JsonSerializer.Serialize(dto, TimelineJsonContext.Default.CreateTimelineEventDto);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("api/timelineevents", content);
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"[TimelineService] CreateAsync falhou. Status: {(int)response.StatusCode}");
                return (false, error);
            }
            return (true, null);
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"[TimelineService] CreateAsync - Erro de rede: {ex.Message}");
            return (false, ex.Message);
        }
        catch (JsonException ex)
        {
            Console.WriteLine($"[TimelineService] CreateAsync - Erro de JSON: {ex.Message}");
            return (false, ex.Message);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[TimelineService] CreateAsync - Erro inesperado: {ex.Message}");
            return (false, ex.Message);
        }
    }

    public async Task<(bool Success, string? Error)> UpdateAsync(UpdateTimelineEventDto dto)
    {
        try
        {
            var json = JsonSerializer.Serialize(dto, TimelineJsonContext.Default.UpdateTimelineEventDto);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync($"api/timelineevents/{dto.Id}", content);
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"[TimelineService] UpdateAsync({dto.Id}) falhou. Status: {(int)response.StatusCode}");
                return (false, error);
            }
            return (true, null);
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"[TimelineService] UpdateAsync - Erro de rede: {ex.Message}");
            return (false, ex.Message);
        }
        catch (JsonException ex)
        {
            Console.WriteLine($"[TimelineService] UpdateAsync - Erro de JSON: {ex.Message}");
            return (false, ex.Message);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[TimelineService] UpdateAsync - Erro inesperado: {ex.Message}");
            return (false, ex.Message);
        }
    }

    public async Task<(bool Success, string? Error)> DeleteAsync(Guid id)
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"api/timelineevents/{id}");
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"[TimelineService] DeleteAsync({id}) falhou. Status: {(int)response.StatusCode}");
                return (false, error);
            }
            return (true, null);
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"[TimelineService] DeleteAsync({id}) - Erro de rede: {ex.Message}");
            return (false, ex.Message);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[TimelineService] DeleteAsync({id}) - Erro inesperado: {ex.Message}");
            return (false, ex.Message);
        }
    }
}