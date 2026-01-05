using PowerShellAutomationAgent.API.Services.Dashboard.Models;

namespace PowerShellAutomationAgent.API.Services.Dashboard;

public interface IDashboardService
{
    public Task<DashboardResponseModel> GetDashboard();
}
