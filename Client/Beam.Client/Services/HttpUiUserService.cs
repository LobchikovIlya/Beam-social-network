using System.Net.Http.Json;
using Beam.Shared.Dto;
using Beam.UI.Interfaces;

namespace Beam.Client.BlazorWasm.Services;

public class HttpUiUserService : IUiUserService
{
    private readonly HttpClient _http;


    public HttpUiUserService(HttpClient http)
    {
        _http = http;
    }

    public async Task<UserDto> GetByIdAsync(Guid id)
    {
        var response = await _http.GetAsync($"api/users/{id}");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<UserDto>();
    }
}