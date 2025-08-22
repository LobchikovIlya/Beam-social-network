namespace Beam.Infrastructure.Entities;

public class UsersToRole
{
    public Guid UserId { get; set; }
    public int RoleId { get; set; }

    public User User { get; set; }
    public Role Role { get; set; }
}