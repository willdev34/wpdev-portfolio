// ====================================
// Título: ContactService.cs
// Descrição: Serviço para consumir endpoints de Contact da API
// Autor: Will
// Empresa: WpDev
// Data: 09/12/2024
// ====================================

using Portfolio.Web.DTOs.Contact;
using System.Net.Http.Json;

namespace Portfolio.Web.Services;

public class ContactService
{
    private readonly HttpClient _httpClient;

    public ContactService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    // Buscar todas as mensagens
    public async Task<List<ContactMessageDto>> GetAllAsync()
    {
        try
        {
            var messages = await _httpClient.GetFromJsonAsync<List<ContactMessageDto>>("api/contact");
            return messages ?? new List<ContactMessageDto>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao buscar mensagens: {ex.Message}");
            return new List<ContactMessageDto>();
        }
    }

    // Buscar mensagem por ID
    public async Task<ContactMessageDto?> GetByIdAsync(Guid id)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<ContactMessageDto>($"api/contact/{id}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao buscar mensagem {id}: {ex.Message}");
            return null;
        }
    }

    // Enviar nova mensagem de contato
    public async Task<bool> SendMessageAsync(ContactMessageDto message)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("api/contact", message);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao enviar mensagem: {ex.Message}");
            return false;
        }
    }
}