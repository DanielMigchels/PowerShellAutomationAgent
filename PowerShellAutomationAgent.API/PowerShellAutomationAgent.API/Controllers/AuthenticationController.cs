using PowerShellAutomationAgent.API.Services.Authentication;
using PowerShellAutomationAgent.API.Services.Authentication.Models;
using Microsoft.AspNetCore.Mvc;

namespace PowerShellAutomationAgent.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthenticationController(IAuthenticationService authenticationService) : ControllerBase
{
    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginRequestModel loginRequestModel)
    {
        var result = authenticationService.Login(loginRequestModel);
        return Ok(result);
    }
}
