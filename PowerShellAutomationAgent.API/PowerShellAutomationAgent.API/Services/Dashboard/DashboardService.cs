using PowerShellAutomationAgent.API.Data;
using PowerShellAutomationAgent.API.Enums;
using PowerShellAutomationAgent.API.Services.Dashboard.Models;
using PowerShellAutomationAgent.API.Services.Jobs.Models;
using Microsoft.EntityFrameworkCore;

namespace PowerShellAutomationAgent.API.Services.Dashboard;

public class DashboardService(DatabaseContext db) : IDashboardService
{
    public async Task<DashboardResponseModel> GetDashboard()
    {
        return new DashboardResponseModel
        {
            FailedJobsCount = await db.Jobs.CountAsync(x => x.State == Enums.JobStates.Failed),
            SuccessfulJobsCount = await db.Jobs.CountAsync(x => x.State == JobStates.Succeeded),
            RunningJobsCount = await db.Jobs.CountAsync(x => x.State == Enums.JobStates.InProgress),
            RecentJobs = await db.Jobs.AsNoTracking()
                .OrderByDescending(x => x.CreatedOnUtc)
                .Take(7)
                .Where(x => x.CreatedOnUtc >= DateTimeOffset.UtcNow.AddDays(-3))
                .Select(x => new JobResponseModel()
                {
                    Id = x.Id,
                    State = x.State,
                    ProjectName = x.Project.Name,
                    HasArtifacts = x.HasArtifacts,
                    CreatedOnUtc = x.CreatedOnUtc,
                    StartedOnUtc = x.StartedOnUtc,
                    FinishedOnUtc = x.FinishedOnUtc
                })
                .ToListAsync()
        };
    }
}
