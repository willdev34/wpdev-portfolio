// ====================================
// Título: NowService.cs
// Descrição: Serviço para consumir endpoints da seção Now da API
// ====================================
using Portfolio.Web.DTOs.Now;
using Portfolio.Web.Json;
using System.Net;
using System.Text;
using System.Text.Json;

namespace Portfolio.Web.Services;

public class NowService
{
    private readonly HttpClient _httpClient;

    public NowService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<NowSectionDto?> GetCurrentAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("api/nowsection");
            if (response.StatusCode == HttpStatusCode.NotFound)
                return null;
            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"[NowService] GetCurrentAsync falhou. Status: {(int)response.StatusCode}");
                return null;
            }
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize(json, NowJsonContext.Default.NowSectionDto);
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"[NowService] GetCurrentAsync - Erro de rede: {ex.Message}");
            return null;
        }
        catch (JsonException ex)
        {
            Console.WriteLine($"[NowService] GetCurrentAsync - Erro de JSON: {ex.Message}");
            return null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[NowService] GetCurrentAsync - Erro inesperado: {ex.Message}");
            return null;
        }
    }

    public async Task<List<NowSectionDto>> GetAllAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("api/nowsections");
            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"[NowService] GetAllAsync falhou. Status: {(int)response.StatusCode}");
                return new List<NowSectionDto>();
            }
            var json = await response.Content.ReadAsStringAsync();
            var sections = JsonSerializer.Deserialize(json, NowJsonContext.Default.ListNowSectionDto);
            return sections ?? new List<NowSectionDto>();
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"[NowService] GetAllAsync - Erro de rede: {ex.Message}");
            return new List<NowSectionDto>();
        }
        catch (JsonException ex)
        {
            Console.WriteLine($"[NowService] GetAllAsync - Erro de JSON: {ex.Message}");
            return new List<NowSectionDto>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[NowService] GetAllAsync - Erro inesperado: {ex.Message}");
            return new List<NowSectionDto>();
        }
    }

    public async Task<NowSectionDto?> GetByIdAsync(Guid id)
    {
        try
        {
            var response = await _httpClient.GetAsync($"api/nowsections/{id}");
            if (response.StatusCode == HttpStatusCode.NotFound)
                return null;
            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"[NowService] GetByIdAsync({id}) falhou. Status: {(int)response.StatusCode}");
                return null;
            }
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize(json, NowJsonContext.Default.NowSectionDto);
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"[NowService] GetByIdAsync({id}) - Erro de rede: {ex.Message}");
            return null;
        }
        catch (JsonException ex)
        {
            Console.WriteLine($"[NowService] GetByIdAsync({id}) - Erro de JSON: {ex.Message}");
            return null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[NowService] GetByIdAsync({id}) - Erro inesperado: {ex.Message}");
            return null;
        }
    }

    public async Task<(bool Success, string? Error)> CreateAsync(CreateNowSectionDto dto)
    {
        try
        {
            var json = JsonSerializer.Serialize(dto, NowJsonContext.Default.CreateNowSectionDto);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("api/nowsections", content);
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"[NowService] CreateAsync falhou. Status: {(int)response.StatusCode}");
                return (false, error);
            }
            return (true, null);
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"[NowService] CreateAsync - Erro de rede: {ex.Message}");
            return (false, ex.Message);
        }
        catch (JsonException ex)
        {
            Console.WriteLine($"[NowService] CreateAsync - Erro de JSON: {ex.Message}");
            return (false, ex.Message);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[NowService] CreateAsync - Erro inesperado: {ex.Message}");
            return (false, ex.Message);
        }
    }

    public async Task<(bool Success, string? Error)> UpdateAsync(UpdateNowSectionDto dto)
    {
        try
        {
            var json = JsonSerializer.Serialize(dto, NowJsonContext.Default.UpdateNowSectionDto);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync($"api/nowsections/{dto.Id}", content);
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"[NowService] UpdateAsync({dto.Id}) falhou. Status: {(int)response.StatusCode}");
                return (false, error);
            }
            return (true, null);
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"[NowService] UpdateAsync - Erro de rede: {ex.Message}");
            return (false, ex.Message);
        }
        catch (JsonException ex)
        {
            Console.WriteLine($"[NowService] UpdateAsync - Erro de JSON: {ex.Message}");
            return (false, ex.Message);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[NowService] UpdateAsync - Erro inesperado: {ex.Message}");
            return (false, ex.Message);
        }
    }

    public async Task<(bool Success, string? Error)> DeleteAsync(Guid id)
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"api/nowsections/{id}");
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"[NowService] DeleteAsync({id}) falhou. Status: {(int)response.StatusCode}");
                return (false, error);
            }
            return (true, null);
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"[NowService] DeleteAsync({id}) - Erro de rede: {ex.Message}");
            return (false, ex.Message);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[NowService] DeleteAsync({id}) - Erro inesperado: {ex.Message}");
            return (false, ex.Message);
        }
    }
}