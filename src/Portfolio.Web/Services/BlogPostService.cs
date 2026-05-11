using Portfolio.Web.DTOs.BlogPosts;
using System.Net.Http.Json;
using System.Text.Json;

namespace Portfolio.Web.Services;

public class BlogPostService
{
    private readonly HttpClient _httpClient;
    
    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public BlogPostService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<BlogPostCardDto>> GetAllAsync()
    {
        try
        {
            var response = await _httpClient.GetFromJsonAsync<List<BlogPostCardDto>>(
                "api/blogposts", _jsonOptions);
            return response ?? new List<BlogPostCardDto>();
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
            return await _httpClient.GetFromJsonAsync<BlogPostDto>(
                $"api/blogposts/{id}", _jsonOptions);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao buscar post {id}: {ex.Message}");
            return null;
        }
    }

    public async Task<List<BlogPostCardDto>> GetPublishedAsync()
    {
        try
        {
            var response = await _httpClient.GetFromJsonAsync<List<BlogPostCardDto>>(
                "api/blogposts", _jsonOptions);
            return response ?? new List<BlogPostCardDto>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao buscar posts publicados: {ex.Message}");
            return new List<BlogPostCardDto>();
        }
    }
}