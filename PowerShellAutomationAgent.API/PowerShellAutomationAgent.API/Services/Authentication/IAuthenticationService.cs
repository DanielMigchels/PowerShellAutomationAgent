using PowerShellAutomationAgent.API.Services.Authentication.Models;

namespace PowerShellAutomationAgent.API.Services.Authentication;

public interface IAuthenticationService
{
    public LoginResponseModel Login(LoginRequestModel loginRequestModel);
}
