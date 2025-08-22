using Beam.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;

namespace Beam.Infrastructure;

public class BeamDbContext : DbContext
{
    public BeamDbContext(DbContextOptions<BeamDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Post> Posts { get; set; }
    public DbSet<PostLike> PostLikes { get; set; }
    public DbSet<Comment> Comments { get; set; }
    public DbSet<CommentLike> CommentLikes { get; set; }

    public DbSet<Role> Roles { get; set; }

    public DbSet<ChatMessage> ChatMessages { get; set; }

    public DbSet<UsersToRole> UsersToRoles { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);


        modelBuilder.ApplyConfigurationsFromAssembly(typeof(BeamDbContext).Assembly);
    }
}