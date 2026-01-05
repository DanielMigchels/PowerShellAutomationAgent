namespace PowerShellAutomationAgent.API.Services.BackgroundJob;

public interface IBackgroundJobQueue
{
    void QueueJob(int jobId);
    bool TryDequeue(out int jobId);
}