using Beam.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;

namespace Beam.Infrastructure;

public class BeamDbContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Post> Posts { get; set; }
    public DbSet<PostLike> PostLikes { get; set; }
    public DbSet<Comment> Comments { get; set; }
    public DbSet<CommentLike> CommentLikes { get; set; }

    public DbSet<Role> Roles { get; set; }

    public DbSet<UsersToRole> UsersToRoles { get; set; }

    public BeamDbContext(DbContextOptions<BeamDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Role>().HasData
        (
            new Role { Id = 1, Name = "Admin" },
            new Role { Id = 2, Name = "User" }
        );

        modelBuilder.Entity<PostLike>().HasKey(pl => new { pl.UserId, pl.PostId });
        modelBuilder.Entity<CommentLike>()
            .HasKey(cl => new { cl.UserId, cl.CommentId });
        modelBuilder.Entity<UsersToRole>()
            .HasKey(ur => new { ur.UserId, ur.RoleId });

        modelBuilder.Entity<UsersToRole>()
            .HasOne(ur => ur.User)
            .WithMany(u => u.UsersToRoles)
            .HasForeignKey(ur => ur.UserId)
            .OnDelete(DeleteBehavior.Cascade);


        modelBuilder.Entity<UsersToRole>()
            .HasOne(ur => ur.Role)
            .WithMany(r => r.UsersToRoles)
            .HasForeignKey(ur => ur.RoleId)
            .OnDelete(DeleteBehavior.Cascade);
        

        modelBuilder.Entity<Post>()
            .HasMany(p => p.Comments) 
            .WithOne(c => c.Post) 
            .HasForeignKey(c => c.PostId) 
            .OnDelete(DeleteBehavior.Cascade);
    }
}