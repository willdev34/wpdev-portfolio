using System.Net;
using System.Net.Http.Json;
using Portfolio.Application.DTOs.ContactMessages;

namespace Portfolio.IntegrationTests;

[Collection("Integration Tests")]
public class ContactMessagesEndpointTests : IntegrationTestBase
{
    public ContactMessagesEndpointTests(CustomWebApplicationFactory factory) : base(factory) { }

    [Fact]
    public async Task Post_ContactMessage_ReturnsCreated_WhenDataIsValid()
    {
        var payload = new CreateContactMessageDto
        {
            Name = "Maria Teste",
            Email = "maria.teste@example.com",
            Subject = "Assunto de teste de integracao",
            Message = "Essa e uma mensagem de teste com mais de dez caracteres.",
            Type = 0
        };

        var response = await Client.PostAsJsonAsync("/api/contactmessages", payload);

        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var created = await response.Content.ReadFromJsonAsync<ContactMessageDto>();
        created.Should().NotBeNull();
        created!.Name.Should().Be(payload.Name);
        created.Email.Should().Be(payload.Email);
        created.Status.Should().Be("New");
    }

    [Fact]
    public async Task Post_ContactMessage_ReturnsBadRequest_WhenNameIsEmpty()
    {
        var payload = new CreateContactMessageDto
        {
            Name = "",
            Email = "valido@example.com",
            Subject = "Assunto valido de teste",
            Message = "Mensagem valida com mais de dez caracteres.",
            Type = 0
        };

        var response = await Client.PostAsJsonAsync("/api/contactmessages", payload);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Post_ContactMessage_ReturnsBadRequest_WhenEmailIsInvalid()
    {
        var payload = new CreateContactMessageDto
        {
            Name = "Nome Valido",
            Email = "isso-nao-e-um-email",
            Subject = "Assunto valido de teste",
            Message = "Mensagem valida com mais de dez caracteres.",
            Type = 0
        };

        var response = await Client.PostAsJsonAsync("/api/contactmessages", payload);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Post_ContactMessage_ReturnsBadRequest_WhenMessageIsTooShort()
    {
        var payload = new CreateContactMessageDto
        {
            Name = "Nome Valido",
            Email = "valido@example.com",
            Subject = "Assunto valido de teste",
            Message = "curta",
            Type = 0
        };

        var response = await Client.PostAsJsonAsync("/api/contactmessages", payload);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}