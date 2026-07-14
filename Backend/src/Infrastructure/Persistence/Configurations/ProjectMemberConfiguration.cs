using AIWorkspace.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AIWorkspace.Infrastructure.Persistence.Configurations;

public class ProjectMemberConfiguration : IEntityTypeConfiguration<ProjectMember>
{
    public void Configure(EntityTypeBuilder<ProjectMember> builder)
    {
        builder.ToTable("ProjectMembers");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Role).IsRequired().HasConversion<string>().HasMaxLength(20);

        builder.Property(x => x.JoinedAt).IsRequired();

        builder
            .HasOne(x => x.Project)
            .WithMany(x => x.ProjectMembers)
            .HasForeignKey(x => x.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => new { x.ProjectId, x.UserId }).IsUnique();
    }
}
