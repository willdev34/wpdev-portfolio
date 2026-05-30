// ====================================
// Título: ContactService.cs
// Descrição: Serviço para consumir endpoints de Contact da API
// Fix produção: usa ContactJsonContext (Source Generators)
//               ao invés de reflection
// Fix rota: api/contactmessages (não api/contact)
// ====================================

using Portfolio.Web.DTOs.Contact;
using Portfolio.Web.Json;
using System.Net;
using System.Text;
using System.Text.Json;

namespace Portfolio.Web.Services;

public class ContactService
{
    private readonly HttpClient _httpClient;

    public ContactService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    // Envia nova mensagem de contato via POST
    // Usa SendContactMessageDto alinhado com a API
    public async Task<bool> SendMessageAsync(SendContactMessageDto message)
    {
        try
        {
            // Serializa usando Source Generator (fix produção)
            var json = JsonSerializer.Serialize(message, ContactJsonContext.Default.SendContactMessageDto);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("api/contactmessages", content);

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"[ContactService] SendMessageAsync falhou. Status: {(int)response.StatusCode}");
                return false;
            }

            return true;
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"[ContactService] SendMessageAsync - Erro de rede: {ex.Message}");
            return false;
        }
        catch (JsonException ex)
        {
            Console.WriteLine($"[ContactService] SendMessageAsync - Erro ao serializar: {ex.Message}");
            return false;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ContactService] SendMessageAsync - Erro inesperado: {ex.Message}");
            return false;
        }
    }

    // Busca todas as mensagens (admin)
    public async Task<List<ContactMessageDto>> GetAllAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("api/contactmessages");

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"[ContactService] GetAllAsync falhou. Status: {(int)response.StatusCode}");
                return new List<ContactMessageDto>();
            }

            var json = await response.Content.ReadAsStringAsync();

            // Source Generator - funciona em produção
            return JsonSerializer.Deserialize(json, ContactJsonContext.Default.ListContactMessageDto)
                   ?? new List<ContactMessageDto>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ContactService] GetAllAsync - Erro: {ex.Message}");
            return new List<ContactMessageDto>();
        }
    }

    // Busca mensagem por ID (admin)
    public async Task<ContactMessageDto?> GetByIdAsync(Guid id)
    {
        try
        {
            var response = await _httpClient.GetAsync($"api/contactmessages/{id}");

            if (response.StatusCode == HttpStatusCode.NotFound)
                return null;

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"[ContactService] GetByIdAsync({id}) falhou. Status: {(int)response.StatusCode}");
                return null;
            }

            var json = await response.Content.ReadAsStringAsync();

            // Source Generator - funciona em produção
            return JsonSerializer.Deserialize(json, ContactJsonContext.Default.ContactMessageDto);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ContactService] GetByIdAsync - Erro: {ex.Message}");
            return null;
        }
    }
}