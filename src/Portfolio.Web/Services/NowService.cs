// ====================================
// Título: NowService.cs
// Descrição: Serviço para consumir endpoints da seção Now da API
// ====================================

using Portfolio.Web.DTOs.Now;
using System.Net;
using System.Text.Json;

namespace Portfolio.Web.Services;

public class NowService
{
    private readonly HttpClient _httpClient;

    public NowService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    // Buscar o conteúdo atual da seção Now
    // É um único registro, então retornamos direto o objeto
    public async Task<NowSectionDto?> GetCurrentAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("api/now");

            // 404 aqui pode significar que ainda não tem conteúdo cadastrado, tudo bem
            if (response.StatusCode == HttpStatusCode.NotFound)
                return null;

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"[NowService] GetCurrentAsync falhou. Status: {(int)response.StatusCode} {response.StatusCode}");
                return null;
            }

            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<NowSectionDto>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"[NowService] GetCurrentAsync - Erro de rede: {ex.Message}");
            return null;
        }
        catch (JsonException ex)
        {
            Console.WriteLine($"[NowService] GetCurrentAsync - Erro ao desserializar JSON: {ex.Message}");
            return null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[NowService] GetCurrentAsync - Erro inesperado: {ex.Message}");
            return null;
        }
    }

    // Buscar por ID caso precise acessar versões específicas
    public async Task<NowSectionDto?> GetByIdAsync(Guid id)
    {
        try
        {
            var response = await _httpClient.GetAsync($"api/now/{id}");

            // 404 é caso esperado, não é bug
            if (response.StatusCode == HttpStatusCode.NotFound)
                return null;

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"[NowService] GetByIdAsync({id}) falhou. Status: {(int)response.StatusCode} {response.StatusCode}");
                return null;
            }

            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<NowSectionDto>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"[NowService] GetByIdAsync({id}) - Erro de rede: {ex.Message}");
            return null;
        }
        catch (JsonException ex)
        {
            Console.WriteLine($"[NowService] GetByIdAsync({id}) - Erro ao desserializar JSON: {ex.Message}");
            return null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[NowService] GetByIdAsync({id}) - Erro inesperado: {ex.Message}");
            return null;
        }
    }
}