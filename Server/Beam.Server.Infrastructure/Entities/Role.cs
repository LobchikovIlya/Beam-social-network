namespace Beam.Infrastructure.Entities;

public class Role
{
    public int Id { get; set; }
    public string Name { get; set; }
    public ICollection<UsersToRole> UsersToRoles { get; set; }
}
