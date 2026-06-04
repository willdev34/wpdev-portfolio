// ====================================
// Título: AppUser.cs
// Descrição: Usuário admin do sistema, estende IdentityUser
// ====================================

using Microsoft.AspNetCore.Identity;

namespace Portfolio.Infrastructure.Data;

public class AppUser : IdentityUser
{
    public string DisplayName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}