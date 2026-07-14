using AIWorkspace.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AIWorkspace.Infrastructure.Persistence.Configurations;

public class TaskItemConfiguration : IEntityTypeConfiguration<TaskItem>
{
    public void Configure(EntityTypeBuilder<TaskItem> builder)
    {
        builder.ToTable("TaskItems");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Title).IsRequired().HasMaxLength(200);

        builder.Property(x => x.Description).HasMaxLength(2000);

        builder.Property(x => x.Priority).IsRequired().HasConversion<string>().HasMaxLength(20);

        builder.Property(x => x.Status).IsRequired().HasConversion<string>().HasMaxLength(20);

        builder.Property(x => x.CreatedAt).IsRequired();

        builder.Property(x => x.DueDate);

        builder
            .HasOne(x => x.Project)
            .WithMany(x => x.TaskItems)
            .HasForeignKey(x => x.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasOne(x => x.AssignedTo)
            .WithMany(x => x.AssignedTaskItems)
            .HasForeignKey(x => x.AssignedToId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
