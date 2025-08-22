using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Security.Claims;
using Beam.Client.BlazorWasm.Services;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
// Не забудьте добавить это пространство имен
// Для использования AuthenticationStateProvider

public class JwtAuthenticationStateProvider : AuthenticationStateProvider
{
    private readonly HttpClient _httpClient;
    private readonly ILocalStorageService _localStorage;

    public JwtAuthenticationStateProvider(ILocalStorageService localStorage, HttpClient httpClient)
    {
        _localStorage = localStorage;
        _httpClient = httpClient;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var token = await _localStorage.GetItemAsync<string>("authToken");
        Console.WriteLine($"Token from storage: {token}");

        ClaimsPrincipal user;

        if (!string.IsNullOrWhiteSpace(token))
        {
            token = token.Trim().Trim('"');
            try
            {
                var identity = new ClaimsIdentity(JwtParser.ParseClaimsFromJwt(token), "jwtAuthType");
                user = new ClaimsPrincipal(identity);

                // Установка заголовка авторизации для HttpClient
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                // Дополнительно можно проверить срок действия токена
                var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);
                var expClaim = jwt.Claims.FirstOrDefault(c => c.Type == "exp")?.Value;
                if (expClaim != null &&
                    DateTimeOffset.FromUnixTimeSeconds(long.Parse(expClaim)) < DateTimeOffset.UtcNow)
                {
                    user = new ClaimsPrincipal(new ClaimsIdentity()); // токен просрочен
                    _httpClient.DefaultRequestHeaders.Authorization = null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"JWT parse error: {ex.Message}");
                user = new ClaimsPrincipal(new ClaimsIdentity());
            }
        }
        else
        {
            user = new ClaimsPrincipal(new ClaimsIdentity());
        }

        return new AuthenticationState(user);
    }

    public void NotifyUserAuthentication(string token)
    {
        var identity = new ClaimsIdentity(JwtParser.ParseClaimsFromJwt(token), "jwtAuthType");
        var user = new ClaimsPrincipal(identity);

        // Установка заголовка авторизации
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
    }

    public void NotifyUserLogout()
    {
        var identity = new ClaimsIdentity();
        var user = new ClaimsPrincipal(identity);

        //     // Удаление заголовка авторизации при выходе
        _httpClient.DefaultRequestHeaders.Authorization = null;

        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
    }
}