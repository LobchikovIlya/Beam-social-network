using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Beam.UI;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");


builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri("http://localhost:5049") // Укажи здесь URL твоего API
});

builder.Services.AddBlazoredLocalStorage();


builder.Services.AddScoped<AuthenticationStateProvider, JwtAuthenticationStateProvider>(); // Регистрация твоего JwtAuthenticationStateProvider


await builder.Build().RunAsync();
