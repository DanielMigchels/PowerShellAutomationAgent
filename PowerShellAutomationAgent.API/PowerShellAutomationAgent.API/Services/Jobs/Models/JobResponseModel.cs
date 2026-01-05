using PowerShellAutomationAgent.API.Enums;

namespace PowerShellAutomationAgent.API.Services.Jobs.Models;

public class JobResponseModel
{
    public int Id { get; set; }
    public JobStates State { get; set; }
    public string? Script { get; set; }
    public bool HasArtifacts { get; set; } = false;
    public string ProjectName { get; set; } = string.Empty;
    public DateTimeOffset CreatedOnUtc { get; set; }
    public DateTimeOffset? StartedOnUtc { get; set; }
    public DateTimeOffset? FinishedOnUtc { get; set; }
    public string? Output { get; internal set; }
}
