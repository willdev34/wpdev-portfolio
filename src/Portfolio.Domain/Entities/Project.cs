namespace Portfolio.Domain.Entities;

public class Project
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? ShortDescription { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public string? DemoUrl { get; set; }
    public string? RepositoryUrl { get; set; }
    public List<string> Technologies { get; set; } = new();
    public int Year { get; set; }
    public bool IsFeatured { get; set; }
    public ProjectStatus Status { get; set; }
    
    // Atributos estilo "carta de jogo" (Yu-Gi-Oh!)
    public CardRarity Rarity { get; set; }
    public int AttackPoints { get; set; }
    public int DefensePoints { get; set; }
    public string? FlavorText { get; set; }
    
    // Auditoria
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public bool IsActive { get; set; } = true;
}

public enum ProjectStatus
{
    Planning = 0,
    InProgress = 1,
    Completed = 2,
    Archived = 3
}

public enum CardRarity
{
    Common = 0,
    Rare = 1,
    SuperRare = 2,
    UltraRare = 3,
    Secret = 4
}