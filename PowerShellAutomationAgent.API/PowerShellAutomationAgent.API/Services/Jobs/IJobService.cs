using PowerShellAutomationAgent.API.Services.Jobs.Models;
using PowerShellAutomationAgent.API.Services.Pagination;

namespace PowerShellAutomationAgent.API.Services.Jobs;

public interface IJobService
{
    public Task<PaginatedList<JobResponseModel>> GetJobs(int pageSize, int page);
    public Task<JobResponseModel?> GetJob(int jobId);
    public Task StartJob(Guid projectId);
    public FileStream DownloadArtifacts(int jobId);
}
