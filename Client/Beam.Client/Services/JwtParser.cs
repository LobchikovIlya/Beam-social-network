using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Beam.Client.BlazorWasm.Services;

public static class JwtParser
{
    public static IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
    {
        var claims = new List<Claim>();
        var jwtHandler = new JwtSecurityTokenHandler();

        // Проверьте, является ли токен действительным
        if (jwtHandler.CanReadToken(jwt))
        {
            var token = jwtHandler.ReadJwtToken(jwt);
            claims.AddRange(token.Claims);
        }

        return claims;
    }
}