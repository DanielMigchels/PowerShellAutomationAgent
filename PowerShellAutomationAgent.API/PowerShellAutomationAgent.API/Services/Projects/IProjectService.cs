using PowerShellAutomationAgent.API.Services.Pagination;
using PowerShellAutomationAgent.API.Services.Projects.Models;

namespace PowerShellAutomationAgent.API.Services.Projects;

public interface IProjectService
{
    public Task<PaginatedList<ProjectResponseModel>> GetProjects(int pageSize = 2147483647, int page = 0);
    public Task<ProjectResponseModel?> GetProject(Guid projectId);
    public Task<AddProjectResponseModel> AddProjects(AddProjectRequestModel requestModel);
    public Task<bool> EditProject(Guid projectId, EditProjectRequestModel requestModel);
    public Task<bool> DeleteProject(Guid projectId);
}
