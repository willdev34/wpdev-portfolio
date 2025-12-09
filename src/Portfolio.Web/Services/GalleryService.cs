// ====================================
// Título: GalleryService.cs
// Descrição: Serviço para consumir endpoints de Gallery da API
// Autor: Will
// Empresa: WpDev
// Data: 09/12/2024
// ====================================

using Portfolio.Web.DTOs.Gallery;
using System.Net.Http.Json;

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
            var images = await _httpClient.GetFromJsonAsync<List<GalleryImageDto>>("api/gallery");
            return images ?? new List<GalleryImageDto>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao buscar imagens: {ex.Message}");
            return new List<GalleryImageDto>();
        }
    }

    // Buscar imagem por ID
    public async Task<GalleryImageDto?> GetByIdAsync(Guid id)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<GalleryImageDto>($"api/gallery/{id}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao buscar imagem {id}: {ex.Message}");
            return null;
        }
    }
}