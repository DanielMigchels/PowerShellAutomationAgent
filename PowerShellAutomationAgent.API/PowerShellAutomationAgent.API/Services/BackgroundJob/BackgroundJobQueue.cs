using System.Collections.Concurrent;

namespace PowerShellAutomationAgent.API.Services.BackgroundJob;

public class BackgroundJobQueue : IBackgroundJobQueue
{
    private readonly ConcurrentQueue<int> _jobs = new();
    public void QueueJob(int jobId) => _jobs.Enqueue(jobId);
    public bool TryDequeue(out int jobId) => _jobs.TryDequeue(out jobId);
}
