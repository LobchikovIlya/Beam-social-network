using System.Net.Http.Json;
using Beam.Client.BlazorWasm.Services;
using Beam.UI;
using Beam.UI.Configuration;
using Beam.UI.Interfaces;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.AspNetCore.SignalR.Client;
using ITokenService = Beam.UI.Interfaces.ITokenService;
using TokenService = Beam.Client.BlazorWasm.Services.TokenService; // Добавлено

var builder = WebAssemblyHostBuilder.CreateDefault(args);
var environment = builder.HostEnvironment.Environment;
var configFile = $"appsettings.{environment}.json";
var httpClient = new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) };
var settings = await httpClient.GetFromJsonAsync<AppSettings>(configFile);

// if(settings == null || string.IsNullOrWhiteSpace(settings.ApiBaseUrl))
// throw new Exception("ApiBaseUrl не задан.");
if (settings == null) settings = new AppSettings();

if (string.IsNullOrWhiteSpace(settings.ApiBaseUrl))
{
    var baseUri = builder.HostEnvironment.BaseAddress;

    settings.ApiBaseUrl = baseUri.Contains("localhost")
        ? "http://localhost:5001"
        : "http://192.168.0.102:5001"; // 👈 замени на свой IP в локальной сети
}

builder.Services.AddSingleton(settings);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");
builder.Services.AddBlazoredLocalStorage();
builder.Services
    .AddScoped<AuthenticationStateProvider,
        JwtAuthenticationStateProvider>(); // Регистрация твоего JwtAuthenticationStateProvider
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IUiUserService, HttpUiUserService>();
builder.Services.AddAuthorizationCore(); // Это необходимо для авторизации
builder.Services.AddScoped<AuthorizationMessageHandler>();
builder.Services.AddScoped<TokenService>();
builder.Services.AddHttpClient("AuthorizedClient", client => { client.BaseAddress = new Uri(settings.ApiBaseUrl); })
    .AddHttpMessageHandler<AuthorizationMessageHandler>();

// Подключаем HttpClient с обработчиком
builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("AuthorizedClient"));
builder.Services.AddScoped(sp =>
{
    var settings = sp.GetRequiredService<AppSettings>();
    var localStorage = sp.GetRequiredService<ILocalStorageService>();

    return new HubConnectionBuilder()
        .WithUrl($"{settings.ApiBaseUrl}/chatHub", options =>
        {
            options.AccessTokenProvider = async () =>
            {
                var token = await localStorage.GetItemAsync<string>("authToken");
                Console.WriteLine($"TOKEN from localStorage = {token}");
                return token;
            };
        })
        .WithAutomaticReconnect()
        .Build();
});
builder.Services.AddScoped<SignalService>();
builder.Services.AddScoped<AuthService>();


await builder.Build().RunAsync();