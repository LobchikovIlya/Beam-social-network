using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Beam.Infrastructure.Entities;

namespace Beam.Client.BlazorWasm.Services;

public class TokenService
{
    public string GetUserNameFromToken(string token)
    {
        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(token);
        var userName = jwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.UniqueName);

        return userName?.Value;
    }
    public string GetUserIdFromToken(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var jwtToken = tokenHandler.ReadJwtToken(token);
        var userIdClaim = jwtToken?.Claims?.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);

        return userIdClaim?.Value; 
    }

}