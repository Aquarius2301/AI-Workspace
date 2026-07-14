using AIWorkspace.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AIWorkspace.Infrastructure.Persistence.Configurations;

public class TeamConfiguration : IEntityTypeConfiguration<Team>
{
    public void Configure(EntityTypeBuilder<Team> builder)
    {
        builder.ToTable("Teams");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Slug).IsRequired().HasMaxLength(200);

        builder.HasIndex(x => x.Slug).IsUnique();

        builder.Property(x => x.Name).IsRequired().HasMaxLength(100);

        builder.Property(x => x.Description).HasMaxLength(500);

        builder
            .HasMany(x => x.TeamMembers)
            .WithOne(x => x.Team)
            .HasForeignKey(x => x.TeamId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasMany(x => x.Projects)
            .WithOne(x => x.Team)
            .HasForeignKey(x => x.TeamId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
