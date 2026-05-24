// ====================================
// Título: ContactService.cs
// Descrição: Serviço para consumir endpoints de Contact da API
// ====================================

using Portfolio.Web.DTOs.Contact;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace Portfolio.Web.Services;

public class ContactService
{
    private readonly HttpClient _httpClient;

    public ContactService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    // Enviar nova mensagem de contato
    // Esse método é diferente: faz POST, não GET
    public async Task<bool> SendMessageAsync(ContactMessageDto message)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("api/contact", message);

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"[ContactService] SendMessageAsync falhou. Status: {(int)response.StatusCode} {response.StatusCode}");
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
            Console.WriteLine($"[ContactService] SendMessageAsync - Erro ao serializar payload: {ex.Message}");
            return false;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ContactService] SendMessageAsync - Erro inesperado: {ex.Message}");
            return false;
        }
    }

    // Buscar todas as mensagens (uso interno/admin no futuro)
    public async Task<List<ContactMessageDto>> GetAllAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("api/contact");

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"[ContactService] GetAllAsync falhou. Status: {(int)response.StatusCode} {response.StatusCode}");
                return new List<ContactMessageDto>();
            }

            var json = await response.Content.ReadAsStringAsync();
            var messages = JsonSerializer.Deserialize<List<ContactMessageDto>>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return messages ?? new List<ContactMessageDto>();
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"[ContactService] GetAllAsync - Erro de rede: {ex.Message}");
            return new List<ContactMessageDto>();
        }
        catch (JsonException ex)
        {
            Console.WriteLine($"[ContactService] GetAllAsync - Erro ao desserializar JSON: {ex.Message}");
            return new List<ContactMessageDto>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ContactService] GetAllAsync - Erro inesperado: {ex.Message}");
            return new List<ContactMessageDto>();
        }
    }

    // Buscar mensagem por ID (uso interno/admin no futuro)
    public async Task<ContactMessageDto?> GetByIdAsync(Guid id)
    {
        try
        {
            var response = await _httpClient.GetAsync($"api/contact/{id}");

            // 404 é caso esperado, não é bug
            if (response.StatusCode == HttpStatusCode.NotFound)
                return null;

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"[ContactService] GetByIdAsync({id}) falhou. Status: {(int)response.StatusCode} {response.StatusCode}");
                return null;
            }

            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<ContactMessageDto>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"[ContactService] GetByIdAsync({id}) - Erro de rede: {ex.Message}");
            return null;
        }
        catch (JsonException ex)
        {
            Console.WriteLine($"[ContactService] GetByIdAsync({id}) - Erro ao desserializar JSON: {ex.Message}");
            return null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ContactService] GetByIdAsync({id}) - Erro inesperado: {ex.Message}");
            return null;
        }
    }
}