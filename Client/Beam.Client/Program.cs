using System.Net.Http.Json;
using Beam.Client.BlazorWasm;

using Beam.Client.BlazorWasm.Services;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Beam.UI;
using Beam.UI.Configuration;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.SignalR.Client;
using TokenService = Beam.Client.BlazorWasm.Services.TokenService; // Добавлено

var builder = WebAssemblyHostBuilder.CreateDefault(args);
var environment = builder.HostEnvironment.Environment;
var configFile = $"appsettings.{environment}.json";

var httpClient = new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) };
var settings = await httpClient.GetFromJsonAsync<AppSettings>(configFile);

if(settings == null || string.IsNullOrWhiteSpace(settings.ApiBaseUrl))
    throw new Exception("ApiBaseUrl не задан.");

builder.Services.AddSingleton(settings);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddBlazoredLocalStorage();
builder.Services.AddScoped<AuthenticationStateProvider, JwtAuthenticationStateProvider>(); // Регистрация твоего JwtAuthenticationStateProvider
builder.Services.AddAuthorizationCore(); // Это необходимо для авторизации
/*builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri("http://localhost:5049") // Укажи здесь URL твоего API
});*/
builder.Services.AddScoped<AuthorizationMessageHandler>();
builder.Services.AddScoped<TokenService>();

builder.Services.AddHttpClient("AuthorizedClient", client =>
    {
        client.BaseAddress = new Uri(settings.ApiBaseUrl);
    })
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
            options.AccessTokenProvider = async () => await localStorage.GetItemAsync<string>("authToken");
            
        })
        .WithAutomaticReconnect()
        .Build();
});


await builder.Build().RunAsync();
