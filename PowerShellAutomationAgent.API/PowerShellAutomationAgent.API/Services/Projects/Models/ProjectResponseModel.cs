namespace PowerShellAutomationAgent.API.Services.Projects.Models;

public class ProjectResponseModel
{
    public DateTimeOffset CreatedOnUtc { get; set; }
    public string Name { get; set; } = string.Empty;
    public Guid Id { get; set; }
    public string Script { get; set; } = string.Empty;
}
