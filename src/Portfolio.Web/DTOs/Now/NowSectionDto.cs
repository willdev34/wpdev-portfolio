// ====================================
// Título: NowSectionDto.cs
// Descrição: DTO para a seção "O que estou fazendo agora"
// Autor: Will
// Empresa: WpDev
// Data: 09/12/2024
// ====================================

namespace Portfolio.Web.DTOs.Now;

public class NowSectionDto
{
    public Guid Id { get; set; }
    public string CurrentFocus { get; set; } = string.Empty;
    public List<string> CurrentlyLearning { get; set; } = new();
    public List<string> CurrentGoals { get; set; } = new();
    public string CurrentLocation { get; set; } = string.Empty;
    public DateTime LastUpdated { get; set; }
}