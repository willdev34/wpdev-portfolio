// Título: ContactMessageDtos.cs
// Descrição: DTOs do frontend para mensagens de contato (admin)

namespace Portfolio.Web.DTOs.ContactMessages;

public class ContactMessageCardDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

public class ContactMessageDetailDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
    public string? ResponseMessage { get; set; }
    public DateTime? RespondedAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class UpdateContactMessageStatusDto
{
    public Guid Id { get; set; }
    // 0=New, 1=Read, 2=Responded, 3=Archived, 4=Spam
    public int Status { get; set; }
    public string? ResponseMessage { get; set; }
}