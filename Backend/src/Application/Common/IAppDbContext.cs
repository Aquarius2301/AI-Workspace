using AIWorkspace.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AIWorkspace.Application.Common;

public interface IAppDbContext
{
    DbSet<User> Users { get; }
    DbSet<Team> Teams { get; }
    DbSet<TeamMember> TeamMembers { get; }
    DbSet<Project> Projects { get; }
    DbSet<ProjectMember> ProjectMembers { get; }
    DbSet<TaskItem> TaskItems { get; }
    DbSet<Picture> Pictures { get; }
    DbSet<RefreshToken> RefreshTokens { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
