using PowerShellAutomationAgent.API.Data;
using PowerShellAutomationAgent.API.Services.Pagination;
using PowerShellAutomationAgent.API.Services.Projects.Models;
using Microsoft.EntityFrameworkCore;

namespace PowerShellAutomationAgent.API.Services.Projects;

public class ProjectService(DatabaseContext db) : IProjectService
{
    public async Task<AddProjectResponseModel> AddProjects(AddProjectRequestModel requestModel)
    {
        var project = new Data.Models.Project()
        {
            Id = Guid.NewGuid(),
            Name = requestModel.Name,
            CreatedOnUtc = DateTimeOffset.UtcNow
        };

        db.Projects.Add(project);
        await db.SaveChangesAsync();

        return new AddProjectResponseModel()
        {
            Id = project.Id
        };
    }

    public async Task<bool> DeleteProject(Guid projectId)
    {
        var project = await db.Projects.Where(x => x.Id == projectId).FirstOrDefaultAsync();
        if (project == null)
        {
            return false;
        }

        db.Remove(project);
        await db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> EditProject(Guid projectId, EditProjectRequestModel requestModel)
    {
        var project = await db.Projects.Where(x => x.Id == projectId).FirstOrDefaultAsync();
        if (project == null)
        {
            return false;
        }

        project.Name = requestModel.Name;
        project.Script = requestModel.Script;
        await db.SaveChangesAsync();
        return true;
    }

    public async Task<ProjectResponseModel?> GetProject(Guid projectId)
    {
        return await db.Projects.AsNoTracking()
            .Where(x => x.Id == projectId)
            .Select(x => new ProjectResponseModel()
            {
                Id = x.Id,
                Name = x.Name,
                CreatedOnUtc = x.CreatedOnUtc,
                Script = x.Script,
            })
            .FirstOrDefaultAsync();
    }

    public async Task<PaginatedList<ProjectResponseModel>> GetProjects(int pageSize = int.MaxValue, int page = 0)
    {
        var data = await db.Projects.AsNoTracking()
            .OrderByDescending(x => x.CreatedOnUtc)
            .Skip(page * pageSize)
            .Take(pageSize)
            .Select(x => new ProjectResponseModel()
            {
                Id = x.Id,
                Name = x.Name,
                CreatedOnUtc = x.CreatedOnUtc
            })
            .ToListAsync();

        return new PaginatedList<ProjectResponseModel>()
        {
            Data = data,
            HasNext = await db.Projects.Skip((page + 1) * pageSize).AnyAsync(),
            HasPrevious = page > 0,
            Page = page,
            PageSize = pageSize
        };
    }
}
