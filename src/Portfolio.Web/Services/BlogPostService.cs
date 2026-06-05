// ====================================
// Título: BlogPostService.cs
// Descrição: Serviço para consumir endpoints de BlogPost da API
// ====================================
using Portfolio.Web.DTOs.BlogPosts;
using Portfolio.Web.Json;
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

    public async Task<List<BlogPostCardDto>> GetAllAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("api/blogposts");
            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"[BlogPostService] GetAllAsync falhou. Status: {(int)response.StatusCode}");
                return new List<BlogPostCardDto>();
            }
            var json = await response.Content.ReadAsStringAsync();
            var posts = JsonSerializer.Deserialize(json, BlogPostJsonContext.Default.ListBlogPostCardDto);
            return posts ?? new List<BlogPostCardDto>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[BlogPostService] GetAllAsync - Erro: {ex.Message}");
            return new List<BlogPostCardDto>();
        }
    }

    public async Task<BlogPostDto?> GetByIdAsync(Guid id)
    {
        try
        {
            var response = await _httpClient.GetAsync($"api/blogposts/{id}");
            if (response.StatusCode == HttpStatusCode.NotFound)
                return null;
            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"[BlogPostService] GetByIdAsync({id}) falhou. Status: {(int)response.StatusCode}");
                return null;
            }
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize(json, BlogPostJsonContext.Default.BlogPostDto);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[BlogPostService] GetByIdAsync({id}) - Erro: {ex.Message}");
            return null;
        }
    }

    public async Task<List<BlogPostCardDto>> GetPublishedAsync()
    {
        return await GetAllAsync();
    }

    // ====================================
    // CREATE (Admin)
    // ====================================
    public async Task<(bool Success, string? Error)> CreateAsync(CreateBlogPostDto dto)
    {
        try
        {
            var json = JsonSerializer.Serialize(dto, BlogPostJsonContext.Default.CreateBlogPostDto);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("api/blogposts", content);
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"[BlogPostService] CreateAsync retornou {(int)response.StatusCode}: {error}");
                return (false, $"Erro {(int)response.StatusCode}: {error}");
            }
            return (true, null);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[BlogPostService] Erro em CreateAsync: {ex.Message}");
            return (false, ex.Message);
        }
    }

    // ====================================
    // UPDATE (Admin)
    // ====================================
    public async Task<(bool Success, string? Error)> UpdateAsync(UpdateBlogPostDto dto)
    {
        try
        {
            var json = JsonSerializer.Serialize(dto, BlogPostJsonContext.Default.UpdateBlogPostDto);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync($"api/blogposts/{dto.Id}", content);
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"[BlogPostService] UpdateAsync({dto.Id}) retornou {(int)response.StatusCode}: {error}");
                return (false, $"Erro {(int)response.StatusCode}: {error}");
            }
            return (true, null);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[BlogPostService] Erro em UpdateAsync: {ex.Message}");
            return (false, ex.Message);
        }
    }

    // ====================================
    // DELETE (Admin)
    // ====================================
    public async Task<(bool Success, string? Error)> DeleteAsync(Guid id)
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"api/blogposts/{id}");
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"[BlogPostService] DeleteAsync({id}) retornou {(int)response.StatusCode}: {error}");
                return (false, $"Erro {(int)response.StatusCode}: {error}");
            }
            return (true, null);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[BlogPostService] Erro em DeleteAsync: {ex.Message}");
            return (false, ex.Message);
        }
    }
}