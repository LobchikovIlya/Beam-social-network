namespace Beam.Client.BlazorWasm.Services;

public class AuthService
{
    public event Func<Task>? OnLogin;
    public event Func<Task>? OnLogout;
    public async Task RaiseOnLogin() =>await OnLogin?.Invoke();
    public async Task RaiseOnLogout() =>await OnLogout?.Invoke();
}