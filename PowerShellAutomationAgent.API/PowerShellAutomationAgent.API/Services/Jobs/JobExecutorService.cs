using PowerShellAutomationAgent.API.Data;
using PowerShellAutomationAgent.API.Data.Models;
using PowerShellAutomationAgent.API.Enums;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Text;

namespace PowerShellAutomationAgent.API.Services.Jobs;

public class JobExecutorService(ILogger<JobExecutorService> logger, DatabaseContext db)
{
    SemaphoreSlim semaphore = new SemaphoreSlim(1, 1);

    public async Task ExecuteJob(int jobId, CancellationToken cancellationToken = default)
    {
        var job = await db.Jobs.FirstOrDefaultAsync(x => x.Id == jobId, cancellationToken: cancellationToken);
        if (job == null)
        {
            logger.LogWarning("Job {JobId} not found", jobId);
            return;
        }

        job.State = JobStates.InProgress;
        job.StartedOnUtc = DateTimeOffset.UtcNow;
        await db.SaveChangesAsync(cancellationToken);

        var script = job.Script.Replace("${jobId}", jobId.ToString());

        var secrets = db.Secrets.ToList();
        foreach (var secret in secrets)
        {
            script = script.Replace($"${{{secret.Key}}}", secret.Value);
        }

        try
        {
            var exitCode = await RunPowerShellScriptAsync(script, job, cancellationToken);

            var artifactsPath = $"C://Artifacts/{jobId}";
            job.HasArtifacts = Directory.Exists(artifactsPath);

            job.State = exitCode == 0 ? JobStates.Succeeded : JobStates.Failed;
        }
        catch (OperationCanceledException)
        {
            job.Output = "Execution cancelled.";
            job.State = JobStates.Failed;
            logger.LogInformation("Job {JobId} was cancelled", jobId);
        }
        catch (Exception ex)
        {
            job.Output = $"Error: {ex.Message}";
            job.State = JobStates.Failed;
            logger.LogError(ex, "Job {JobId} failed", jobId);
        }
        finally
        {
            job.FinishedOnUtc = DateTimeOffset.UtcNow;

            await semaphore.WaitAsync();
            try
            {
                db.Update(job);
                await db.SaveChangesAsync();

            }
            finally
            {
                semaphore.Release();
            }
        }
    }


    private async Task<int> RunPowerShellScriptAsync(string script, Job job, CancellationToken cancellationToken)
    {
        var tempFilePath = Path.Combine(Path.GetTempPath(), $"jobscript_{Guid.NewGuid()}.ps1");
        var outputFilePath = Path.Combine(Path.GetTempPath(), $"output_{Guid.NewGuid()}.txt");

        await File.WriteAllTextAsync(tempFilePath, script, cancellationToken);

        var arguments = $"-NoProfile -ExecutionPolicy Bypass -File  \"{tempFilePath}\"";
        var psi = new ProcessStartInfo
        {
            FileName = "powershell.exe",
            Arguments = arguments,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true,
            StandardOutputEncoding = Encoding.UTF8,
            StandardErrorEncoding = Encoding.UTF8
        };

        using var process = new Process { StartInfo = psi };

        process.OutputDataReceived += async (sender, e) =>
        {
            if (e.Data == null) return;

            logger.LogInformation(e.Data);
            job.Output += e.Data + Environment.NewLine;

            await semaphore.WaitAsync();
            try
            {
                db.Update(job);
                await db.SaveChangesAsync();
                
            }
            finally
            {
                semaphore.Release();
            }
        };

        process.ErrorDataReceived += async (sender, e) =>
        {
            if (e.Data == null) return;

            logger.LogWarning(e.Data);
            job.Output += e.Data + Environment.NewLine;

            await semaphore.WaitAsync();
            try
            {
                db.Update(job);
                await db.SaveChangesAsync();
            }
            finally
            {
                semaphore.Release();
            }
        };

        process.Start();
        process.BeginOutputReadLine();
        process.BeginErrorReadLine();

        await process.WaitForExitAsync(cancellationToken);

        try { File.Delete(tempFilePath); } catch { }

        return process.ExitCode;
    }
}