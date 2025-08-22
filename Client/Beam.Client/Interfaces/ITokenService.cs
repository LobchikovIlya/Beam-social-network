namespace Beam.UI.Interfaces;

public interface ITokenService
{
    string? GetUserNameFromToken(string token);
    string? GetUserIdFromToken(string token);
    public Task<string?> GetValidTokenAsync();
}