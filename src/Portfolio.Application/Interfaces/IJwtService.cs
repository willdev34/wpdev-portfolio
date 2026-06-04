// ====================================
// Título: IJwtService.cs
// Descrição: Interface para geração de tokens JWT
// ====================================

namespace Portfolio.Application.Interfaces;

public interface IJwtService
{
    string GenerateToken(string userId, string email, string displayName);
    DateTime GetExpiration();
}