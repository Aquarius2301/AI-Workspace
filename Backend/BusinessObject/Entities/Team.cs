namespace BusinessObject.Entities;

public class Team
{
    /// <summary>
    /// Gets or sets the unique identifier of the team.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the name of the team.
    /// </summary>
    public string Name { get; set; } = null!;

    /// <summary>
    /// Gets or sets the optional description of the team.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the team members associated with this team.
    /// </summary>
    public ICollection<TeamMember> TeamMembers { get; set; } = new List<TeamMember>();

    /// <summary>
    /// Gets or sets the projects owned by this team.
    /// </summary>
    public ICollection<Project> Projects { get; set; } = new List<Project>();
}
