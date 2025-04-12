using Beam.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Beam.Infrastructure.EntityConfigurations;

public class PostConfiguration : IEntityTypeConfiguration<Post>
{
    public void Configure(EntityTypeBuilder<Post> builder)
    {
        builder.HasKey(p => p.Id);

        builder.HasOne(p => p.User)  // Связь с пользователем
            .WithMany(u => u.Posts)  // Пользователь может иметь много постов
            .HasForeignKey(p => p.UserId)  // Внешний ключ для связи
            .OnDelete(DeleteBehavior.Cascade);  // Поведение при удалении

        builder.HasMany(p => p.Comments)  // Связь с комментариями
            .WithOne(c => c.Post)  // Один пост может иметь много комментариев
            .HasForeignKey(c => c.PostId)  // Внешний ключ для связи
            .OnDelete(DeleteBehavior.Cascade);  // Поведение при удалении

        builder.HasMany(p => p.Likes)  // Связь с лайками
            .WithOne(pl => pl.Post)  // Один пост может иметь много лайков
            .HasForeignKey(pl => pl.PostId)  // Внешний ключ для связи
            .OnDelete(DeleteBehavior.Cascade);  // Поведение при удалении
    }
}