// Título: ContactMessageService.cs
// Descrição: Service para consumir a API de mensagens de contato no admin

using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Portfolio.Web.DTOs.ContactMessages;

namespace Portfolio.Web.Services;

// Source Generators — obrigatório em Release (sem reflection)
[JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
[JsonSerializable(typeof(List<ContactMessageCardDto>))]
[JsonSerializable(typeof(ContactMessageDetailDto))]
[JsonSerializable(typeof(UpdateContactMessageStatusDto))]
public partial class ContactMessageJsonContext : JsonSerializerContext { }

public class ContactMessageService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<ContactMessageService> _logger;

    public ContactMessageService(HttpClient httpClient, ILogger<ContactMessageService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    // Busca todas as mensagens (admin)
    public async Task<List<ContactMessageCardDto>> GetAllAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("api/contactmessages");

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                _logger.LogWarning("[ContactMessageService] Não autorizado em GetAllAsync");
                return new List<ContactMessageCardDto>();
            }

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("[ContactMessageService] GetAllAsync retornou {Status}", response.StatusCode);
                return new List<ContactMessageCardDto>();
            }

            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize(json, ContactMessageJsonContext.Default.ListContactMessageCardDto)
                   ?? new List<ContactMessageCardDto>();
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "[ContactMessageService] Erro HTTP em GetAllAsync");
            return new List<ContactMessageCardDto>();
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "[ContactMessageService] Erro JSON em GetAllAsync");
            return new List<ContactMessageCardDto>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[ContactMessageService] Erro inesperado em GetAllAsync");
            return new List<ContactMessageCardDto>();
        }
    }

    // Busca detalhe de uma mensagem (admin)
    public async Task<ContactMessageDetailDto?> GetByIdAsync(Guid id)
    {
        try
        {
            var response = await _httpClient.GetAsync($"api/contactmessages/{id}");

            if (response.StatusCode == HttpStatusCode.NotFound) return null;

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("[ContactMessageService] GetByIdAsync retornou {Status}", response.StatusCode);
                return null;
            }

            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize(json, ContactMessageJsonContext.Default.ContactMessageDetailDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[ContactMessageService] Erro em GetByIdAsync");
            return null;
        }
    }

    // Marca como lida (Status = 1) ou desmarca (Status = 0)
    public async Task<bool> ToggleReadAsync(Guid id, bool markAsRead)
    {
        try
        {
            var dto = new UpdateContactMessageStatusDto
            {
                Id = id,
                Status = markAsRead ? 1 : 0  // 1=Read, 0=New
            };

            var json = JsonSerializer.Serialize(dto, ContactMessageJsonContext.Default.UpdateContactMessageStatusDto);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync($"api/contactmessages/{id}", content);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("[ContactMessageService] ToggleReadAsync retornou {Status}", response.StatusCode);
                return false;
            }

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[ContactMessageService] Erro em ToggleReadAsync");
            return false;
        }
    }

    // Deleta uma mensagem (admin)
    public async Task<bool> DeleteAsync(Guid id)
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"api/contactmessages/{id}");

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("[ContactMessageService] DeleteAsync retornou {Status}", response.StatusCode);
                return false;
            }

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[ContactMessageService] Erro em DeleteAsync");
            return false;
        }
    }

    public async Task<int> GetUnreadCountAsync()
    {
        var messages = await GetAllAsync();
        return messages.Count(m => m.Status == "New");
    }
}