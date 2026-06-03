// ====================================
// Título: GalleryService.cs
// Descrição: Serviço para consumir endpoints de Gallery da API
// Fix produção: usa GalleryJsonContext (Source Generators)
// Fix rota: api/galleryimages
// ====================================

using Portfolio.Web.DTOs.Gallery;
using Portfolio.Web.Json;
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

    // Busca todas as imagens ordenadas por DisplayOrder
    public async Task<List<GalleryImageDto>> GetAllAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("api/galleryimages");

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"[GalleryService] GetAllAsync falhou. Status: {(int)response.StatusCode}");
                return new List<GalleryImageDto>();
            }

            var json = await response.Content.ReadAsStringAsync();

            // Source Generator - funciona em produção (sem reflection)
            return JsonSerializer.Deserialize(json, GalleryJsonContext.Default.ListGalleryImageDto)
                   ?? new List<GalleryImageDto>();
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"[GalleryService] GetAllAsync - Erro de rede: {ex.Message}");
            return new List<GalleryImageDto>();
        }
        catch (JsonException ex)
        {
            Console.WriteLine($"[GalleryService] GetAllAsync - Erro ao desserializar: {ex.Message}");
            return new List<GalleryImageDto>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[GalleryService] GetAllAsync - Erro inesperado: {ex.Message}");
            return new List<GalleryImageDto>();
        }
    }

    // Busca imagem por ID
    public async Task<GalleryImageDto?> GetByIdAsync(Guid id)
    {
        try
        {
            var response = await _httpClient.GetAsync($"api/galleryimages/{id}");

            if (response.StatusCode == HttpStatusCode.NotFound)
                return null;

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"[GalleryService] GetByIdAsync({id}) falhou. Status: {(int)response.StatusCode}");
                return null;
            }

            var json = await response.Content.ReadAsStringAsync();

            // Source Generator - funciona em produção (sem reflection)
            return JsonSerializer.Deserialize(json, GalleryJsonContext.Default.GalleryImageDto);
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"[GalleryService] GetByIdAsync - Erro de rede: {ex.Message}");
            return null;
        }
        catch (JsonException ex)
        {
            Console.WriteLine($"[GalleryService] GetByIdAsync - Erro ao desserializar: {ex.Message}");
            return null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[GalleryService] GetByIdAsync - Erro inesperado: {ex.Message}");
            return null;
        }
    }
}