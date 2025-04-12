using System.Net.Http.Headers; // Не забудьте добавить это пространство имен
using System.Security.Claims;
using System.Threading.Tasks;
using Beam.Client.BlazorWasm.Services;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization; // Для использования AuthenticationStateProvider


public class JwtAuthenticationStateProvider : AuthenticationStateProvider
{
    private readonly ILocalStorageService _localStorage;
    private readonly HttpClient _httpClient;

    public JwtAuthenticationStateProvider(ILocalStorageService localStorage, HttpClient httpClient)
    {
        _localStorage = localStorage;
        _httpClient = httpClient;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var token = await _localStorage.GetItemAsync<string>("authToken");
        var identity = new ClaimsIdentity();

        if (!string.IsNullOrWhiteSpace(token))
        {
            // Установка заголовка авторизации
            //_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var claims = JwtParser.ParseClaimsFromJwt(token); // Парсим токен для извлечения клеймов
            identity = new ClaimsIdentity(claims, "jwtAuthType");
        }

        var user = new ClaimsPrincipal(identity);
        return new AuthenticationState(user);
    }

    public void NotifyUserAuthentication(string token)
    {
        var identity = new ClaimsIdentity(JwtParser.ParseClaimsFromJwt(token), "jwtAuthType");
        var user = new ClaimsPrincipal(identity);

        // Установка заголовка авторизации
       // _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
    }

    public void NotifyUserLogout()
    {
        var identity = new ClaimsIdentity();
        var user = new ClaimsPrincipal(identity);
        
        // Удаление заголовка авторизации при выходе
        _httpClient.DefaultRequestHeaders.Authorization = null;

        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
    }
}
