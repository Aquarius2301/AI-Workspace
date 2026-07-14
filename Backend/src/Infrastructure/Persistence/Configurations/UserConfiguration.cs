using AIWorkspace.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AIWorkspace.Infrastructure.Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name).IsRequired().HasMaxLength(100);

        builder.Property(x => x.Email).IsRequired().HasMaxLength(256);

        builder.HasIndex(x => x.Email).IsUnique();

        builder.Property(x => x.PasswordHash).IsRequired().HasMaxLength(256);

        builder.Property(x => x.Language).IsRequired().HasConversion<string>().HasMaxLength(10);

        builder.Property(x => x.CreatedAt).IsRequired();

        builder.Property(x => x.LastActiveAt);

        builder
            .HasOne(x => x.AvatarPicture)
            .WithOne(x => x.UsedByUser)
            .HasForeignKey<User>(x => x.AvatarPictureId)
            .OnDelete(DeleteBehavior.SetNull);

        builder
            .HasMany(x => x.RefreshTokens)
            .WithOne(x => x.User)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasMany(x => x.TeamMembers)
            .WithOne(x => x.User)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasMany(x => x.AssignedTaskItems)
            .WithOne(x => x.AssignedTo)
            .HasForeignKey(x => x.AssignedToId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
