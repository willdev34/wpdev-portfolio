// ====================================
// Título: BlogPostService.cs
// Descrição: Service para comunicação com API de BlogPosts
// Autor: Will
// Empresa: WpDev
// Data: 09/12/2024
// ====================================

using Portfolio.Web.DTOs.BlogPosts;
using System.Net.Http.Json;

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
            var response = await _httpClient.GetFromJsonAsync<List<BlogPostCardDto>>("api/blogposts");
            return response ?? new List<BlogPostCardDto>();
        }
        catch
        {
            return new List<BlogPostCardDto>();
        }
    }

    public async Task<BlogPostDto?> GetByIdAsync(Guid id)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<BlogPostDto>($"api/blogposts/{id}");
        }
        catch
        {
            return null;
        }
    }

    public async Task<List<BlogPostCardDto>> GetPublishedAsync()
    {
        try
        {
            var response = await _httpClient.GetFromJsonAsync<List<BlogPostCardDto>>("api/blogposts/published");
            return response ?? new List<BlogPostCardDto>();
        }
        catch
        {
            return new List<BlogPostCardDto>();
        }
    }
}