using Portfolio.Web.DTOs.BlogPosts;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

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
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"JSON recebido: {json.Substring(0, Math.Min(100, json.Length))}");
            var posts = JsonSerializer.Deserialize<List<BlogPostCardDto>>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            });
            return posts ?? new List<BlogPostCardDto>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao buscar posts: {ex.Message}");
            return new List<BlogPostCardDto>();
        }
    }

    public async Task<BlogPostDto?> GetByIdAsync(Guid id)
    {
        try
        {
            var response = await _httpClient.GetAsync($"api/blogposts/{id}");
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<BlogPostDto>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao buscar post {id}: {ex.Message}");
            return null;
        }
    }

    public async Task<List<BlogPostCardDto>> GetPublishedAsync()
    {
        return await GetAllAsync();
    }
}