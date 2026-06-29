using BusinessObject.Entities;
using BusinessObject.Enums;
using Microsoft.EntityFrameworkCore;

namespace BusinessObject;

public class AIWorkspaceContext : DbContext
{
    public AIWorkspaceContext(DbContextOptions<AIWorkspaceContext> options)
        : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<Team> Teams => Set<Team>();
    public DbSet<TeamMember> TeamMembers => Set<TeamMember>();
    public DbSet<Project> Projects => Set<Project>();
    public DbSet<ProjectMember> ProjectMembers => Set<ProjectMember>();
    public DbSet<TaskItem> TaskItems => Set<TaskItem>();
    public DbSet<Document> Documents => Set<Document>();
    public DbSet<Comment> Comments => Set<Comment>();
    public DbSet<Attachment> Attachments => Set<Attachment>();
    public DbSet<AiInteraction> AiInteractions => Set<AiInteraction>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure all DateTimeOffset properties to be stored as datetimeoffset in SQL Server
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            foreach (var property in entityType.GetProperties())
            {
                if (
                    property.ClrType == typeof(DateTimeOffset)
                    || property.ClrType == typeof(DateTimeOffset?)
                )
                {
                    property.SetColumnType("datetimeoffset");
                }
            }
        }

        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("Users");
            entity.HasKey(x => x.Id);

            entity.Property(x => x.Id).ValueGeneratedNever();
            entity.Property(x => x.Name).IsRequired().HasMaxLength(200);
            entity.Property(x => x.Email).IsRequired().HasMaxLength(256);
            entity.Property(x => x.AvatarUrl).HasMaxLength(1000);
            entity.Property(x => x.CreatedAt).IsRequired().HasColumnType("datetimeoffset");
            entity.Property(x => x.Language).IsRequired().HasConversion<string>().HasMaxLength(3);

            entity.HasIndex(x => x.Email).IsUnique();

            entity
                .HasMany(x => x.RefreshTokens)
                .WithOne(x => x.User)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity
                .HasMany(x => x.TeamMembers)
                .WithOne(x => x.User)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity
                .HasMany(x => x.AssignedTaskItems)
                .WithOne(x => x.AssignedTo)
                .HasForeignKey(x => x.AssignedToId)
                .OnDelete(DeleteBehavior.SetNull);

            entity
                .HasMany(x => x.Comments)
                .WithOne(x => x.Creator)
                .HasForeignKey(x => x.CreatorId)
                .OnDelete(DeleteBehavior.Restrict);

            entity
                .HasMany(x => x.AiInteractions)
                .WithOne(x => x.User)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.ToTable("RefreshTokens");
            entity.HasKey(x => x.Id);

            entity.Property(x => x.Id).ValueGeneratedNever();
            entity.Property(x => x.Token).IsRequired().HasMaxLength(500);
            entity.Property(x => x.CreatedAt).IsRequired().HasColumnType("datetimeoffset");
            entity.Property(x => x.ExpiresAt).IsRequired().HasColumnType("datetimeoffset");
            entity.Property(x => x.DeviceInfo).HasMaxLength(500);
        });

        modelBuilder.Entity<Team>(entity =>
        {
            entity.ToTable("Teams");
            entity.HasKey(x => x.Id);

            entity.Property(x => x.Id).ValueGeneratedNever();
            entity.Property(x => x.Name).IsRequired().HasMaxLength(250);
            entity.Property(x => x.Description).HasMaxLength(1000);
        });

        modelBuilder.Entity<TeamMember>(entity =>
        {
            entity.ToTable("TeamMembers");
            entity.HasKey(x => x.Id);

            entity.Property(x => x.Id).ValueGeneratedNever();
            entity.Property(x => x.Role).IsRequired().HasConversion<string>().HasMaxLength(50);
            entity.Property(x => x.JoinedAt).IsRequired().HasColumnType("datetimeoffset");

            entity
                .HasOne(x => x.Team)
                .WithMany(x => x.TeamMembers)
                .HasForeignKey(x => x.TeamId)
                .OnDelete(DeleteBehavior.Cascade);

            entity
                .HasOne(x => x.User)
                .WithMany(x => x.TeamMembers)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(x => new { x.TeamId, x.UserId }).IsUnique();
        });

        modelBuilder.Entity<Project>(entity =>
        {
            entity.ToTable("Projects");
            entity.HasKey(x => x.Id);

            entity.Property(x => x.Id).ValueGeneratedNever();
            entity.Property(x => x.Name).IsRequired().HasMaxLength(250);
            entity.Property(x => x.Description).HasMaxLength(2000);
            entity
                .Property(x => x.Visibility)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(50);

            entity
                .HasOne(x => x.Team)
                .WithMany(x => x.Projects)
                .HasForeignKey(x => x.TeamId)
                .OnDelete(DeleteBehavior.Cascade);

            entity
                .HasOne(x => x.Creator)
                .WithMany()
                .HasForeignKey(x => x.CreatorId)
                .OnDelete(DeleteBehavior.Restrict);

            entity
                .HasMany(x => x.ProjectMembers)
                .WithOne(x => x.Project)
                .HasForeignKey(x => x.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<ProjectMember>(entity =>
        {
            entity.ToTable("ProjectMembers");
            entity.HasKey(x => x.Id);

            entity.Property(x => x.Id).ValueGeneratedNever();
            entity.Property(x => x.JoinedAt).IsRequired().HasColumnType("datetimeoffset");

            entity
                .HasOne(x => x.Project)
                .WithMany(x => x.ProjectMembers)
                .HasForeignKey(x => x.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);

            entity
                .HasOne(x => x.User)
                .WithMany()
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(x => new { x.ProjectId, x.UserId }).IsUnique();
        });

        modelBuilder.Entity<TaskItem>(entity =>
        {
            entity.ToTable("TaskItems");
            entity.HasKey(x => x.Id);

            entity.Property(x => x.Id).ValueGeneratedNever();
            entity.Property(x => x.Title).IsRequired().HasMaxLength(500);
            entity.Property(x => x.Description).HasColumnType("nvarchar(max)");
            entity.Property(x => x.Priority).IsRequired();
            entity.Property(x => x.Status).IsRequired().HasConversion<string>().HasMaxLength(50);
            entity.Property(x => x.CreatedAt).IsRequired().HasColumnType("datetimeoffset");
            entity.Property(x => x.DueDate).HasColumnType("datetimeoffset");

            entity
                .HasOne(x => x.Project)
                .WithMany(x => x.TaskItems)
                .HasForeignKey(x => x.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);

            entity
                .HasOne(x => x.AssignedTo)
                .WithMany(x => x.AssignedTaskItems)
                .HasForeignKey(x => x.AssignedToId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<Document>(entity =>
        {
            entity.ToTable("Documents");
            entity.HasKey(x => x.Id);

            entity.Property(x => x.Id).ValueGeneratedNever();
            entity.Property(x => x.Title).IsRequired().HasMaxLength(500);
            entity.Property(x => x.Content).HasColumnType("nvarchar(max)");
            entity.Property(x => x.CreatedAt).IsRequired().HasColumnType("datetimeoffset");

            entity
                .HasOne(x => x.Project)
                .WithMany(x => x.Documents)
                .HasForeignKey(x => x.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);

            entity
                .HasOne(x => x.Creator)
                .WithMany()
                .HasForeignKey(x => x.CreatorId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Comment>(entity =>
        {
            entity.ToTable("Comments");
            entity.HasKey(x => x.Id);

            entity.Property(x => x.Id).ValueGeneratedNever();
            entity.Property(x => x.ReferenceType).IsRequired().HasMaxLength(50);
            entity.Property(x => x.Content).IsRequired().HasMaxLength(2000);
            entity.Property(x => x.CreatedAt).IsRequired().HasColumnType("datetimeoffset");

            entity
                .HasOne(x => x.Creator)
                .WithMany(x => x.Comments)
                .HasForeignKey(x => x.CreatorId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Attachment>(entity =>
        {
            entity.ToTable("Attachments");
            entity.HasKey(x => x.Id);

            entity.Property(x => x.Id).ValueGeneratedNever();
            entity.Property(x => x.ReferenceType).IsRequired().HasMaxLength(50);
            entity.Property(x => x.FileName).IsRequired().HasMaxLength(255);
            entity.Property(x => x.ContentType).HasMaxLength(100);
            entity.Property(x => x.Url).IsRequired().HasMaxLength(1000);
            entity.Property(x => x.UploadedAt).IsRequired().HasColumnType("datetimeoffset");
        });

        modelBuilder.Entity<AiInteraction>(entity =>
        {
            entity.ToTable("AiInteractions");
            entity.HasKey(x => x.Id);

            entity.Property(x => x.Id).ValueGeneratedNever();
            entity.Property(x => x.Model).IsRequired().HasMaxLength(100);
            entity.Property(x => x.Prompt).IsRequired().HasColumnType("nvarchar(max)");
            entity.Property(x => x.ResponseText).IsRequired().HasColumnType("nvarchar(max)");
            entity.Property(x => x.CreatedAt).IsRequired().HasColumnType("datetimeoffset");

            entity
                .HasOne(x => x.User)
                .WithMany(x => x.AiInteractions)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.SetNull);

            entity
                .HasOne(x => x.Project)
                .WithMany(x => x.AiInteractions)
                .HasForeignKey(x => x.ProjectId)
                .OnDelete(DeleteBehavior.SetNull);
        });
    }
}
