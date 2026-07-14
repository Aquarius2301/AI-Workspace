using AIWorkspace.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AIWorkspace.Infrastructure.Persistence.Configurations;

public class TeamMemberConfiguration : IEntityTypeConfiguration<TeamMember>
{
    public void Configure(EntityTypeBuilder<TeamMember> builder)
    {
        builder.ToTable("TeamMembers");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Role).IsRequired().HasConversion<string>().HasMaxLength(20);

        builder.Property(x => x.JoinedAt).IsRequired();

        builder
            .HasOne(x => x.Team)
            .WithMany(x => x.TeamMembers)
            .HasForeignKey(x => x.TeamId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasOne(x => x.User)
            .WithMany(x => x.TeamMembers)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => new { x.TeamId, x.UserId }).IsUnique();
    }
}
