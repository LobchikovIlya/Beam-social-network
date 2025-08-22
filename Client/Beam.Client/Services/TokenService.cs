using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Beam.UI.Interfaces;
using Blazored.LocalStorage;

namespace Beam.Client.BlazorWasm.Services;

public class TokenService : ITokenService
{
    private readonly ILocalStorageService _localStorage;

    public TokenService(ILocalStorageService localStorage)
    {
        _localStorage = localStorage;
    }
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
    public async Task<string?> GetValidTokenAsync()
    {
        string? token = null;

        while (string.IsNullOrEmpty(token))
        {
            token = await _localStorage.GetItemAsync<string>("authToken");
            if (string.IsNullOrEmpty(token))
                await Task.Delay(50);
        }

        // Обрезаем лишние кавычки
        token = token.Trim().Trim('"');

        // Здесь можно добавить проверку срока действия токена, если нужно
        try
        {
            var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);
            var expClaim = jwt.Claims.FirstOrDefault(c => c.Type == "exp")?.Value;
            if (expClaim != null &&
                DateTimeOffset.FromUnixTimeSeconds(long.Parse(expClaim)) < DateTimeOffset.UtcNow)
            {
                return null; // токен просрочен
            }
        }
        catch
        {
            return null; // ошибка парсинга
        }

        return token;
    }
}




