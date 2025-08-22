using Beam.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Beam.Infrastructure.EntityConfigurations;

public class CommentlikeConfiguration : IEntityTypeConfiguration<CommentLike>
{
    public void Configure(EntityTypeBuilder<CommentLike> builder)
    {
        builder.HasKey(cl => new { cl.UserId, cl.CommentId });

        builder.HasOne(cl => cl.Comment)
            .WithMany(c => c.CommentLikes)
            .HasForeignKey(cl => cl.CommentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(cl => cl.User)
            .WithMany(u => u.CommentLikes)
            .HasForeignKey(cl => cl.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}