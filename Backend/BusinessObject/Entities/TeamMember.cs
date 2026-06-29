using BusinessObject.Enums;

namespace BusinessObject.Entities;

public class TeamMember
{
    /// <summary>
    /// Gets or sets the unique identifier of the team member record.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the unique identifier of the related team.
    /// </summary>
    public Guid TeamId { get; set; }

    /// <summary>
    /// Gets or sets the team associated with this membership.
    /// </summary>
    public Team Team { get; set; } = null!;

    /// <summary>
    /// Gets or sets the unique identifier of the related user.
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Gets or sets the user associated with this membership.
    /// </summary>
    public User User { get; set; } = null!;

    /// <summary>
    /// Gets or sets the role of the user within the team.
    /// </summary>
    public TeamMemberRole Role { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the user joined the team.
    /// </summary>
    public DateTimeOffset JoinedAt { get; set; }
}
