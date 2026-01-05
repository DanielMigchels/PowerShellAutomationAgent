using PowerShellAutomationAgent.API.Services.Jobs.Models;

namespace PowerShellAutomationAgent.API.Services.Dashboard.Models;

public class DashboardResponseModel
{
    public int RunningJobsCount { get; set; }
    public int FailedJobsCount { get; set; }
    public int SuccessfulJobsCount { get; set; }
    public IEnumerable<JobResponseModel> RecentJobs { get; set; } = [];
}
