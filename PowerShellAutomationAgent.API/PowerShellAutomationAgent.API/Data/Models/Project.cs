using System.ComponentModel.DataAnnotations;

namespace PowerShellAutomationAgent.API.Data.Models;

public class Project
{
    public Guid Id { get; set; } = Guid.NewGuid();

    [MaxLength(256)]
    public string Name { get; set; } = string.Empty;

    public string Script { get; set; } = string.Empty;

    public DateTimeOffset CreatedOnUtc { get; set; }

    public ICollection<Job> Jobs { get; set; } = [];
}
