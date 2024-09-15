using Beam.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
namespace Beam.Infrastructure;

public class BeamDbContext : DbContext
{ 
    public DbSet<User> Users { get; set; }
    public BeamDbContext(DbContextOptions<BeamDbContext> options) : base(options)
    {
    }
   
}