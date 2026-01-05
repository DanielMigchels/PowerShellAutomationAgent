using PowerShellAutomationAgent.API.Services.Jobs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IO;

namespace PowerShellAutomationAgent.API.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class JobController(IJobService jobService) : ControllerBase
{
    [HttpGet]
    [Authorize]
    public async Task<IActionResult> Get([FromQuery] int pageSize = 2147483647, [FromQuery] int page = 0)
    {
        var result = await jobService.GetJobs(pageSize, page);
        return Ok(result);
    }

    [HttpGet("{jobId}")]
    [Authorize]
    public async Task<IActionResult> Get([FromRoute] int jobId)
    {
        var result = await jobService.GetJob(jobId);
        return Ok(result);
    }

    [HttpPost("{projectId}/start")]
    [Authorize]
    public async Task<IActionResult> Start([FromRoute] Guid projectId)
    {
        await jobService.StartJob(projectId);
        return Ok();
    }

    [HttpGet("{jobId}/artifacts")]
    [Authorize]
    public IActionResult DownloadArtifacts([FromRoute] int jobId)
    {
        var fileStream = jobService.DownloadArtifacts(jobId);
        return File(fileStream, "application/zip", $"{jobId}-artifacts.zip");
    }
}
