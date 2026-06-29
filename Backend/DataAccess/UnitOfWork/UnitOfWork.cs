using BusinessObject;
using BusinessObject.Entities;
using DataAccess.Repositories;

namespace DataAccess.UnitOfWork;

public class UnitOfWork : IUnitOfWork
{
    private readonly AIWorkspaceContext _context;

    public IRepository<AiInteraction> AiInteractions { get; }
    public IRepository<Attachment> Attachments { get; }
    public IRepository<Comment> Comments { get; }
    public IRepository<Document> Documents { get; }
    public IRepository<Project> Projects { get; }
    public IRepository<ProjectMember> ProjectMembers { get; }
    public IRepository<TaskItem> TaskItems { get; }
    public IRepository<Team> Teams { get; }
    public IRepository<TeamMember> TeamMembers { get; }
    public IRepository<User> Users { get; }
    public IRepository<RefreshToken> RefreshTokens { get; }

    public UnitOfWork(
        AIWorkspaceContext context,
        IRepository<AiInteraction> aiInteractions,
        IRepository<Attachment> attachments,
        IRepository<Comment> comments,
        IRepository<Document> documents,
        IRepository<Project> projects,
        IRepository<ProjectMember> projectMembers,
        IRepository<TaskItem> taskItems,
        IRepository<Team> teams,
        IRepository<TeamMember> teamMembers,
        IRepository<User> users,
        IRepository<RefreshToken> refreshTokens
    )
    {
        _context = context;
        AiInteractions = aiInteractions;
        Attachments = attachments;
        Comments = comments;
        Documents = documents;
        Projects = projects;
        ProjectMembers = projectMembers;
        TaskItems = taskItems;
        Teams = teams;
        TeamMembers = teamMembers;
        Users = users;
        RefreshTokens = refreshTokens;
    }

    public async Task BeginTransactionAsync()
    {
        await _context.Database.BeginTransactionAsync();
    }

    public async Task CommitTransactionAsync()
    {
        await _context.Database.CommitTransactionAsync();
    }

    public async Task RollbackTransactionAsync()
    {
        await _context.Database.RollbackTransactionAsync();
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }
}
