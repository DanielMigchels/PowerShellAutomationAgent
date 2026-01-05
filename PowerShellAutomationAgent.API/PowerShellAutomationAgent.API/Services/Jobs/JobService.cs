using PowerShellAutomationAgent.API.Data;
using PowerShellAutomationAgent.API.Data.Models;
using PowerShellAutomationAgent.API.Services.BackgroundJob;
using PowerShellAutomationAgent.API.Services.Jobs.Models;
using PowerShellAutomationAgent.API.Services.Pagination;
using Microsoft.EntityFrameworkCore;
using System.IO.Compression;

namespace PowerShellAutomationAgent.API.Services.Jobs;

public class JobService(DatabaseContext db, IBackgroundJobQueue backgroundJobQueue) : IJobService
{
    public async Task<PaginatedList<JobResponseModel>> GetJobs(int pageSize, int page)
    {
        var data = await db.Jobs.AsNoTracking()
            .OrderByDescending(x => x.Id)
            .Skip(page * pageSize)
            .Take(pageSize)
            .Select(x => new JobResponseModel()
            {
                Id = x.Id,
                State = x.State,
                ProjectName = x.Project.Name,
                HasArtifacts = x.HasArtifacts,
                CreatedOnUtc = x.CreatedOnUtc,
                StartedOnUtc = x.StartedOnUtc,
                FinishedOnUtc = x.FinishedOnUtc,
            })
            .ToListAsync();

        return new PaginatedList<JobResponseModel>()
        {
            Data = data,
            HasNext = await db.Jobs.Skip((page + 1) * pageSize).AnyAsync(),
            HasPrevious = page > 0,
            Page = page,
            PageSize = pageSize
        };
    }

    public async Task<JobResponseModel?> GetJob(int jobId)
    {
        var secrets = await db.Secrets.AsNoTracking().ToListAsync();

        return await db.Jobs.AsNoTracking()
            .Where(x => x.Id == jobId)
            .Select(x => new JobResponseModel()
            {
                Id = x.Id,
                State = x.State,
                Script = x.Script,
                Output = SanitizeOutput(x.Output, secrets),
                ProjectName = x.Project.Name,
                HasArtifacts = x.HasArtifacts,
                CreatedOnUtc = x.CreatedOnUtc,
                StartedOnUtc = x.StartedOnUtc,
                FinishedOnUtc = x.FinishedOnUtc,
            })
            .FirstOrDefaultAsync();
    }

    private static string SanitizeOutput(string? output, List<Secret> secrets)
    {
        if (output == null) return string.Empty;

        foreach (var secret in secrets)
        {
            output = output.Replace(secret.Value, secret.Key);
        }
        
        return output;
    }

    public async Task StartJob(Guid projectId)
    {
        var project = await db.Projects.FirstOrDefaultAsync(x => x.Id == projectId);

        if (project == null)
        {
            return;
        }

        var job = new Job()
        {
            State = Enums.JobStates.Queued,
            Script = project.Script,
            CreatedOnUtc = DateTimeOffset.UtcNow,
            Project = project
        };

        var addedJob = db.Jobs.Add(job);
        await db.SaveChangesAsync();

        backgroundJobQueue.QueueJob(addedJob.Entity.Id);
    }

    public FileStream DownloadArtifacts(int jobId)
    {
        string artifactsPath = $"C://Artifacts/{jobId}/";
        string zipPath = $"C://Artifacts/{jobId}.zip";

        if (!Directory.Exists(artifactsPath))
        {
            var job = db.Jobs.FirstOrDefault(x => x.Id == jobId) ?? throw new DirectoryNotFoundException($"Artifacts directory for job {jobId} not found.");
            job.HasArtifacts = false;
            db.SaveChanges();
            throw new DirectoryNotFoundException($"Artifacts directory for job {jobId} not found.");
        }       

        if (File.Exists(zipPath))
        {
            File.Delete(zipPath);
        }

        ZipFile.CreateFromDirectory(artifactsPath, zipPath, CompressionLevel.Fastest, false);

        return new FileStream(zipPath, FileMode.Open, FileAccess.Read, FileShare.Read);
    }
}
