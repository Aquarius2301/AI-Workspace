namespace BusinessObject.Entities;

public class ProjectMember
{
    /// <summary>
    /// Gets or sets the unique identifier of the project member record.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the unique identifier of the related project.
    /// </summary>
    public Guid ProjectId { get; set; }

    /// <summary>
    /// Gets or sets the project associated with this membership.
    /// </summary>
    public Project Project { get; set; } = null!;

    /// <summary>
    /// Gets or sets the unique identifier of the related user.
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Gets or sets the user associated with this membership.
    /// </summary>
    public User User { get; set; } = null!;

    /// <summary>
    /// Gets or sets the date and time when the user joined the project.
    /// </summary>
    public DateTimeOffset JoinedAt { get; set; }
}
