// ====================================
// Título: BlogPostService.cs
// Descrição: Serviço para consumir endpoints de BlogPost da API
// ====================================

using Portfolio.Web.DTOs.BlogPosts;
using System.Net;
using System.Text.Json;

namespace Portfolio.Web.Services;

public class BlogPostService
{
    private readonly HttpClient _httpClient;

    public BlogPostService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    // Buscar todos os posts (chamado por Blog.razor)
    public async Task<List<BlogPostCardDto>> GetAllAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("api/blogposts");

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"[BlogPostService] GetAllAsync falhou. Status: {(int)response.StatusCode} {response.StatusCode}");
                return new List<BlogPostCardDto>();
            }

            var json = await response.Content.ReadAsStringAsync();
            var posts = JsonSerializer.Deserialize<List<BlogPostCardDto>>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return posts ?? new List<BlogPostCardDto>();
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"[BlogPostService] GetAllAsync - Erro de rede: {ex.Message}");
            return new List<BlogPostCardDto>();
        }
        catch (JsonException ex)
        {
            Console.WriteLine($"[BlogPostService] GetAllAsync - Erro ao desserializar JSON: {ex.Message}");
            return new List<BlogPostCardDto>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[BlogPostService] GetAllAsync - Erro inesperado: {ex.Message}");
            return new List<BlogPostCardDto>();
        }
    }

    // Buscar post por ID (chamado por BlogPostDetail.razor)
    public async Task<BlogPostDto?> GetByIdAsync(Guid id)
    {
        try
        {
            var response = await _httpClient.GetAsync($"api/blogposts/{id}");

            // 404 é caso esperado (ID inválido na URL), sem log de erro
            if (response.StatusCode == HttpStatusCode.NotFound)
                return null;

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"[BlogPostService] GetByIdAsync({id}) falhou. Status: {(int)response.StatusCode} {response.StatusCode}");
                return null;
            }

            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<BlogPostDto>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"[BlogPostService] GetByIdAsync({id}) - Erro de rede: {ex.Message}");
            return null;
        }
        catch (JsonException ex)
        {
            Console.WriteLine($"[BlogPostService] GetByIdAsync({id}) - Erro ao desserializar JSON: {ex.Message}");
            return null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[BlogPostService] GetByIdAsync({id}) - Erro inesperado: {ex.Message}");
            return null;
        }
    }
}