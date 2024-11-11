using System.IdentityModel.Tokens.Jwt;

namespace Beam.Client.BlazorWasm.Services;

public class TokenService
{
    public string GetUserNameFromToken(string token)
    {
        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(token);
        var userName = jwtToken.Claims.FirstOrDefault(claim =>
            claim.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name")?.Value;

        return userName;
    }
}