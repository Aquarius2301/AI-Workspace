using BusinessObject.Enums;

namespace BusinessObject.Entities;

public class User
{
    /// <summary>
    /// Gets or sets the unique identifier of the user.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the display name of the user.
    /// </summary>
    public string Name { get; set; } = null!;

    /// <summary>
    /// Gets or sets the email address of the user.
    /// </summary>
    public string Email { get; set; } = null!;

    /// <summary>
    /// Gets or sets the SHA256 password hash of the user.
    /// </summary>
    public string PasswordHash { get; set; } = null!;

    /// <summary>
    /// Gets or sets the optional avatar URL of the user.
    /// </summary>
    public string? AvatarUrl { get; set; }

    /// <summary>
    /// Gets or sets the language to display
    /// </summary>
    public LanguageDisplay Language { get; set; } = LanguageDisplay.vi;

    /// <summary>
    /// Gets or sets the date and time when the user was created.
    /// </summary>
    public DateTimeOffset CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the user last performed an activity.
    /// </summary>
    public DateTimeOffset? LastActiveAt { get; set; }

    /// <summary>
    /// Gets or sets the refresh tokens associated with the user.
    /// </summary>
    public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();

    /// <summary>
    /// Gets or sets the team memberships of the user.
    /// </summary>
    public ICollection<TeamMember> TeamMembers { get; set; } = new List<TeamMember>();

    /// <summary>
    /// Gets or sets the task items assigned to the user.
    /// </summary>
    public ICollection<TaskItem> AssignedTaskItems { get; set; } = new List<TaskItem>();

    /// <summary>
    /// Gets or sets the comments created by the user.
    /// </summary>
    public ICollection<Comment> Comments { get; set; } = new List<Comment>();

    /// <summary>
    /// Gets or sets the AI interactions created by the user.
    /// </summary>
    public ICollection<AiInteraction> AiInteractions { get; set; } = new List<AiInteraction>();
}
