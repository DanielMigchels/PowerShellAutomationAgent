using PowerShellAutomationAgent.API.Services.Jobs;

namespace PowerShellAutomationAgent.API.Services.BackgroundJob;

public class JobWorkerService(IBackgroundJobQueue queue, IServiceScopeFactory scopeFactory, ILogger<JobWorkerService> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            if (queue.TryDequeue(out var jobId))
            {
                using var scope = scopeFactory.CreateScope();
                var executor = scope.ServiceProvider.GetRequiredService<JobExecutorService>();

                try
                {
                    await executor.ExecuteJob(jobId, stoppingToken);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Failed executing job {JobId}", jobId);
                }
            }
            else
            {
                await Task.Delay(500, stoppingToken);
            }
        }
    }
}
