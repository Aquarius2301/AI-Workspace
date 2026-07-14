using AIWorkspace.Domain.Enums;

namespace AIWorkspace.Domain.Entities;

public class TaskItem
{
    /// <summary>
    /// Gets or sets the unique identifier of the task item.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the unique identifier of the project that contains this task item.
    /// </summary>
    public Guid ProjectId { get; set; }

    /// <summary>
    /// Gets or sets the project that contains this task item.
    /// </summary>
    public Project Project { get; set; } = null!;

    /// <summary>
    /// Gets or sets the title of the task item.
    /// </summary>
    public string Title { get; set; } = null!;

    /// <summary>
    /// Gets or sets the optional detailed description of the task item.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the optional unique identifier of the user assigned to this task item.
    /// </summary>
    public Guid? AssignedToId { get; set; }

    /// <summary>
    /// Gets or sets the user assigned to this task item.
    /// </summary>
    public User? AssignedTo { get; set; }

    /// <summary>
    /// Gets or sets the priority of the task item.
    /// </summary>
    public TaskPriority Priority { get; set; }

    /// <summary>
    /// Gets or sets the current status of the task item.
    /// </summary>
    public TaskItemStatus Status { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the task item was created.
    /// </summary>
    public DateTimeOffset CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the optional due date of the task item.
    /// </summary>
    public DateTimeOffset? DueDate { get; set; }
}
