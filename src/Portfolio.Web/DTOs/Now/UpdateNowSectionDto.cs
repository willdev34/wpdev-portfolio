namespace Portfolio.Web.DTOs.Now;

public class UpdateNowSectionDto
{
    public Guid Id { get; set; }
    public string Content { get; set; } = string.Empty;
    public List<ProjectLink> CurrentProjects { get; set; } = new();
    public List<string> CurrentlyLearning { get; set; } = new();
    public List<string> CurrentGoals { get; set; } = new();
    public bool IsActive { get; set; }
}