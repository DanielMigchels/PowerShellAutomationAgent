namespace PowerShellAutomationAgent.API.Data.Models;

public class Secret
{
    public Guid Id { get; set; }
    public string Key { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
}
