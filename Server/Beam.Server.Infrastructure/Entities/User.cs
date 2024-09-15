namespace Beam.Infrastructure.Entities;

public class User
{
    public Guid Id { get; set; }
    public string Tag { get; set; }
    public string PasswordHash { get; set; }
    public string Name { get; set; }
    public DateTimeOffset CreationDate { get; set; }
    
}