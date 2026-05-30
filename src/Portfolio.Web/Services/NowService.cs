// ====================================
// Título: NowService.cs
// Descrição: Serviço para consumir endpoints da seção Now da API
// Fix produção: usa NowJsonContext (Source Generators) 
//              ao invés de reflection
// ====================================

using Portfolio.Web.DTOs.Now;
using Portfolio.Web.Json;
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

    // Busca o conteúdo atual da seção Now
    // Endpoint retorna um único objeto (não lista)
    public async Task<NowSectionDto?> GetCurrentAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("api/nowsection");

            // 404 significa que ainda não tem conteúdo cadastrado
            if (response.StatusCode == HttpStatusCode.NotFound)
                return null;

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"[NowService] GetCurrentAsync falhou. Status: {(int)response.StatusCode}");
                return null;
            }

            var json = await response.Content.ReadAsStringAsync();

            // Source Generator - funciona em produção (sem reflection)
            return JsonSerializer.Deserialize(json, NowJsonContext.Default.NowSectionDto);
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"[NowService] Erro de rede: {ex.Message}");
            return null;
        }
        catch (JsonException ex)
        {
            Console.WriteLine($"[NowService] Erro ao desserializar JSON: {ex.Message}");
            return null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[NowService] Erro inesperado: {ex.Message}");
            return null;
        }
    }

    // Busca por ID para versões específicas
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

            // Source Generator - funciona em produção (sem reflection)
            return JsonSerializer.Deserialize(json, NowJsonContext.Default.NowSectionDto);
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"[NowService] GetByIdAsync - Erro de rede: {ex.Message}");
            return null;
        }
        catch (JsonException ex)
        {
            Console.WriteLine($"[NowService] GetByIdAsync - Erro ao desserializar JSON: {ex.Message}");
            return null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[NowService] GetByIdAsync - Erro inesperado: {ex.Message}");
            return null;
        }
    }
}