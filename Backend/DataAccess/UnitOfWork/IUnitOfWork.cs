using BusinessObject.Entities;
using DataAccess.Repositories;

namespace DataAccess.UnitOfWork;

/// <summary>
/// Represents a unit of work that coordinates repositories and database transactions.
/// </summary>
public interface IUnitOfWork
{
    /// <summary>
    /// Gets the repository for AI interactions.
    /// </summary>
    IRepository<AiInteraction> AiInteractions { get; }

    /// <summary>
    /// Gets the repository for attachments.
    /// </summary>
    IRepository<Attachment> Attachments { get; }

    /// <summary>
    /// Gets the repository for comments.
    /// </summary>
    IRepository<Comment> Comments { get; }

    /// <summary>
    /// Gets the repository for documents.
    /// </summary>
    IRepository<Document> Documents { get; }

    /// <summary>
    /// Gets the repository for projects.
    /// </summary>
    IRepository<Project> Projects { get; }

    /// <summary>
    /// Gets the repository for project memberships.
    /// </summary>
    IRepository<ProjectMember> ProjectMembers { get; }

    /// <summary>
    /// Gets the repository for task items.
    /// </summary>
    IRepository<TaskItem> TaskItems { get; }

    /// <summary>
    /// Gets the repository for teams.
    /// </summary>
    IRepository<Team> Teams { get; }

    /// <summary>
    /// Gets the repository for team memberships.
    /// </summary>
    IRepository<TeamMember> TeamMembers { get; }

    /// <summary>
    /// Gets the repository for users.
    /// </summary>
    IRepository<User> Users { get; }

    /// <summary>
    /// Gets the repository for refresh tokens.
    /// </summary>
    IRepository<RefreshToken> RefreshTokens { get; }

    /// <summary>
    /// Begins a database transaction asynchronously.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task BeginTransactionAsync();

    /// <summary>
    /// Commits the current database transaction asynchronously.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task CommitTransactionAsync();

    /// <summary>
    /// Rolls back the current database transaction asynchronously.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task RollbackTransactionAsync();

    /// <summary>
    /// Persists all pending changes to the database asynchronously.
    /// </summary>
    /// <returns>The number of state entries written to the database.</returns>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
