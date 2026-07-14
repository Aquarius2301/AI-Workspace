using AIWorkspace.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AIWorkspace.Infrastructure.Persistence.Configurations;

public class ProjectConfiguration : IEntityTypeConfiguration<Project>
{
    public void Configure(EntityTypeBuilder<Project> builder)
    {
        builder.ToTable("Projects");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Slug).IsRequired().HasMaxLength(200);

        builder.HasIndex(x => x.Slug).IsUnique();

        builder.Property(x => x.Name).IsRequired().HasMaxLength(100);

        builder.Property(x => x.Description).HasMaxLength(500);

        builder.Property(x => x.Visibility).IsRequired().HasConversion<string>().HasMaxLength(20);

        builder
            .HasOne(x => x.Team)
            .WithMany(x => x.Projects)
            .HasForeignKey(x => x.TeamId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasOne(x => x.Creator)
            .WithMany()
            .HasForeignKey(x => x.CreatorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasMany(x => x.TaskItems)
            .WithOne(x => x.Project)
            .HasForeignKey(x => x.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasMany(x => x.ProjectMembers)
            .WithOne(x => x.Project)
            .HasForeignKey(x => x.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
