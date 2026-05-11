using Portfolio.Web.DTOs.BlogPosts;
using Portfolio.Web.Json;
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
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            var posts = JsonSerializer.Deserialize(json, BlogPostJsonContext.Default.ListBlogPostCardDto);
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
            return JsonSerializer.Deserialize(json, BlogPostJsonContext.Default.BlogPostDto);
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