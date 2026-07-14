using AIWorkspace.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AIWorkspace.Infrastructure.Persistence.Configurations;

public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.ToTable("RefreshTokens");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Token).IsRequired().HasMaxLength(500);

        builder.HasIndex(x => x.Token).IsUnique();

        builder.Property(x => x.CreatedAt).IsRequired();

        builder.Property(x => x.ExpiresAt).IsRequired();

        builder.Property(x => x.DeviceInfo).HasMaxLength(256);

        builder.Property(x => x.DeviceId).HasMaxLength(100);

        builder
            .HasOne(x => x.User)
            .WithMany(x => x.RefreshTokens)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
