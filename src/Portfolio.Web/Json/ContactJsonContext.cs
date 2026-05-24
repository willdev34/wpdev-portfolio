using System.Text.Json.Serialization;
using Portfolio.Web.DTOs.Contact;

namespace Portfolio.Web.Json;

[JsonSerializable(typeof(List<ContactMessageDto>))]
[JsonSerializable(typeof(ContactMessageDto))]
[JsonSourceGenerationOptions(
    PropertyNameCaseInsensitive = true,
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
)]
public partial class ContactJsonContext : JsonSerializerContext
{
}