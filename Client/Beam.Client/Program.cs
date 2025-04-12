using Beam.Application.Services;
using Beam.Application.Services.Interfaces;
using Beam.Client.BlazorWasm.Services;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Beam.UI;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.SignalR.Client;
using System.Net.Http;  // Добавлено
using Microsoft.Extensions.DependencyInjection;
using TokenService = Beam.Client.BlazorWasm.Services.TokenService; // Добавлено

var builder = WebAssemblyHostBuilder.CreateDefault(args);
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
        client.BaseAddress = new Uri("http://localhost:5049");
    })
    .AddHttpMessageHandler<AuthorizationMessageHandler>();

// Подключаем HttpClient с обработчиком
builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("AuthorizedClient"));




builder.Services.AddSingleton(sp =>
{
    var navigationManager = sp.GetRequiredService<NavigationManager>();
    var localStorage = sp.GetRequiredService<ILocalStorageService>();
    
    return new HubConnectionBuilder()
        .WithUrl(navigationManager.ToAbsoluteUri("http://localhost:5049/chatHub"), options =>
        {
            options.AccessTokenProvider = async () =>
            {
                // Получение токена из LocalStorage
                return await localStorage.GetItemAsync<string>("authToken");
            };
        })
        .WithAutomaticReconnect()
        .Build();
});


await builder.Build().RunAsync();
