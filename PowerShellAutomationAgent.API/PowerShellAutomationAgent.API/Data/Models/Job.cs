using PowerShellAutomationAgent.API.Enums;

namespace PowerShellAutomationAgent.API.Data.Models;

public class Job
{
    public int Id { get; set; }
    public JobStates State { get; set; }
    public required string Script { get; set; }
    public string? Output { get; set; }
    public bool HasArtifacts { get; set; } = false;
    public Guid ProjectId { get; set; }
    public required Project Project { get; set; }
    public DateTimeOffset CreatedOnUtc { get; set; }
    public DateTimeOffset? StartedOnUtc { get; set; }
    public DateTimeOffset? FinishedOnUtc { get; set; }
}
