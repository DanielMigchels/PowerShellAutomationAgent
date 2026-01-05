using PowerShellAutomationAgent.API.Options;
using PowerShellAutomationAgent.API.Services.Authentication.Models;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace PowerShellAutomationAgent.API.Services.Authentication;

public class AuthenticationService(IOptions<JwtOptions> jwtOptions, IOptions<AccessOptions> accessOptions) : IAuthenticationService
{
    public LoginResponseModel Login(LoginRequestModel loginRequestModel)
    {
        if (loginRequestModel.Password != accessOptions.Value.Password)
        {
            return new LoginResponseModel()
            {
                Success = false,
            };
        }

        return new LoginResponseModel()
        {
            Success = true,
            Jwt = GenerateJwtToken()
        };
    }

    private string GenerateJwtToken()
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(jwtOptions.Value.Secret);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Expires = DateTime.UtcNow.AddHours(9),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}
