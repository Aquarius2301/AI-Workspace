using AIWorkspace.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AIWorkspace.Infrastructure.Persistence.Configurations;

public class PictureConfiguration : IEntityTypeConfiguration<Picture>
{
    public void Configure(EntityTypeBuilder<Picture> builder)
    {
        builder.ToTable("Pictures");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.FileId).IsRequired().HasMaxLength(100);

        builder.Property(x => x.Url).IsRequired().HasMaxLength(500);

        builder.Property(x => x.IsActive).IsRequired();

        builder.Property(x => x.CreatedAt).IsRequired();

        builder
            .HasOne(x => x.UsedByUser)
            .WithOne(x => x.AvatarPicture)
            .HasForeignKey<Picture>(x => x.UsedByUserId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
