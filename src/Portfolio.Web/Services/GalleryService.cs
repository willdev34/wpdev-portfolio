// ====================================
// Título: GalleryService.cs
// Descrição: Serviço para consumir endpoints de Gallery da API
// ====================================

using Portfolio.Web.DTOs.Gallery;
using System.Net;
using System.Text.Json;

namespace Portfolio.Web.Services;

public class GalleryService
{
    private readonly HttpClient _httpClient;

    public GalleryService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    // Buscar todas as imagens
    public async Task<List<GalleryImageDto>> GetAllAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("api/gallery");

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"[GalleryService] GetAllAsync falhou. Status: {(int)response.StatusCode} {response.StatusCode}");
                return new List<GalleryImageDto>();
            }

            var json = await response.Content.ReadAsStringAsync();
            var images = JsonSerializer.Deserialize<List<GalleryImageDto>>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return images ?? new List<GalleryImageDto>();
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"[GalleryService] GetAllAsync - Erro de rede: {ex.Message}");
            return new List<GalleryImageDto>();
        }
        catch (JsonException ex)
        {
            Console.WriteLine($"[GalleryService] GetAllAsync - Erro ao desserializar JSON: {ex.Message}");
            return new List<GalleryImageDto>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[GalleryService] GetAllAsync - Erro inesperado: {ex.Message}");
            return new List<GalleryImageDto>();
        }
    }

    // Buscar imagem por ID
    public async Task<GalleryImageDto?> GetByIdAsync(Guid id)
    {
        try
        {
            var response = await _httpClient.GetAsync($"api/gallery/{id}");

            // 404 é caso esperado, não é bug
            if (response.StatusCode == HttpStatusCode.NotFound)
                return null;

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"[GalleryService] GetByIdAsync({id}) falhou. Status: {(int)response.StatusCode} {response.StatusCode}");
                return null;
            }

            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<GalleryImageDto>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"[GalleryService] GetByIdAsync({id}) - Erro de rede: {ex.Message}");
            return null;
        }
        catch (JsonException ex)
        {
            Console.WriteLine($"[GalleryService] GetByIdAsync({id}) - Erro ao desserializar JSON: {ex.Message}");
            return null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[GalleryService] GetByIdAsync({id}) - Erro inesperado: {ex.Message}");
            return null;
        }
    }
}