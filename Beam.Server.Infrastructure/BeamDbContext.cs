using Microsoft.EntityFrameworkCore;
namespace Beam.Infrastructure;

public class BeamDbContext : DbContext
{
    public BeamDbContext(DbContextOptions<BeamDbContext> options) : base(options)
    {
    }
}