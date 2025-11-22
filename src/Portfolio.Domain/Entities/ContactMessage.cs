namespace Portfolio.Domain.Entities;

public class ContactMessage
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public ContactMessageType Type { get; set; }
    public ContactMessageStatus Status { get; set; }
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
    
    // Resposta (se houver)
    public string? ResponseMessage { get; set; }
    public DateTime? RespondedAt { get; set; }
    
    // Auditoria
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public enum ContactMessageType
{
    General = 0,
    JobOpportunity = 1,
    Freelance = 2,
    Partnership = 3,
    Other = 4
}

public enum ContactMessageStatus
{
    New = 0,
    Read = 1,
    Responded = 2,
    Archived = 3,
    Spam = 4
}