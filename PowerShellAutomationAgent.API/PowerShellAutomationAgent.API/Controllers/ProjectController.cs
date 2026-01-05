using PowerShellAutomationAgent.API.Services.Projects;
using PowerShellAutomationAgent.API.Services.Projects.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace PowerShellAutomationAgent.API.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class ProjectController(IProjectService projectService) : ControllerBase
{
    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetProjects([FromQuery] int pageSize = 2147483647, [FromQuery] int page = 0)
    {
        var result = await projectService.GetProjects(pageSize, page);
        return Ok(result);
    }

    [HttpGet("{projectId}")]
    [Authorize]
    public async Task<IActionResult> GetProject([FromRoute] Guid projectId)
    {
        var result = await projectService.GetProject(projectId);
        return Ok(result);
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> AddProject([FromBody] AddProjectRequestModel requestModel)
    {
        var result = await projectService.AddProjects(requestModel);
        return Ok(result);
    }

    [HttpPut("{projectId}")]
    [Authorize]
    public async Task<IActionResult> EditProject([FromRoute] Guid projectId, [FromBody] EditProjectRequestModel requestModel)
    {
        var result = await projectService.EditProject(projectId, requestModel);
        return result ? Ok(result) : BadRequest();
    }

    [HttpDelete("{projectId}")]
    [Authorize]
    public async Task<IActionResult> DeleteProject([FromRoute] Guid projectId)
    {
        var result = await projectService.DeleteProject(projectId);
        return result ? Ok() : BadRequest();
    }
}
