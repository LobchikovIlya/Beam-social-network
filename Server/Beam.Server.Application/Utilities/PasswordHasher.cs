namespace Beam.Application.Utilities;

public  static class PasswordHasher
{
    public static string HashPassword(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password);
    }
}