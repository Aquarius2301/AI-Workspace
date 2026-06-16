using BusinessObject.Enums;

namespace BusinessObject.Entities;

public class Project
{
    /// <summary>
    /// Gets or sets the unique identifier of the project.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the unique identifier of the team that owns the project.
    /// </summary>
    public Guid TeamId { get; set; }

    /// <summary>
    /// Gets or sets the team that owns this project.
    /// </summary>
    public Team Team { get; set; } = null!;

    /// <summary>
    /// Gets or sets the user ID of the creator of the project.
    /// </summary>
    public Guid CreatorId { get; set; }

    /// <summary>
    /// Gets or sets the user who created the project.
    /// </summary>
    public User Creator { get; set; } = null!;

    /// <summary>
    /// Gets or sets the name of the project.
    /// </summary>
    public string Name { get; set; } = null!;

    /// <summary>
    /// Gets or sets the optional description of the project.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the visibility of the project.
    /// </summary>
    public ProjectVisibility Visibility { get; set; } = ProjectVisibility.Private;

    /// <summary>
    /// Gets or sets the task items that belong to this project.
    /// </summary>
    public ICollection<TaskItem> TaskItems { get; set; } = new List<TaskItem>();

    /// <summary>
    /// Gets or sets the documents that belong to this project.
    /// </summary>
    public ICollection<Document> Documents { get; set; } = new List<Document>();

    /// <summary>
    /// Gets or sets the AI interactions associated with this project.
    /// </summary>
    public ICollection<AiInteraction> AiInteractions { get; set; } = new List<AiInteraction>();

    /// <summary>
    /// Gets or sets the project members associated with this project.
    /// </summary>
    public ICollection<ProjectMember> ProjectMembers { get; set; } = new List<ProjectMember>();
}
