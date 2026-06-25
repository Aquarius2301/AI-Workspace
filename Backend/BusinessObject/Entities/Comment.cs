using BusinessObject.Enums;

namespace BusinessObject.Entities;

public class Comment
{
    /// <summary>
    /// Gets or sets the unique identifier of the comment.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the unique identifier of the creator of the comment.
    /// </summary>
    public Guid CreatorId { get; set; }

    /// <summary>
    /// Gets or sets the creator who wrote this comment.
    /// </summary>
    public User Creator { get; set; } = null!;

    /// <summary>
    /// Gets or sets the type of entity this comment references.
    /// </summary>
    public ReferenceType ReferenceType { get; set; } = ReferenceType.Document;

    /// <summary>
    /// Gets or sets the unique identifier of the referenced entity.
    /// </summary>
    public Guid ReferenceId { get; set; }

    /// <summary>
    /// Gets or sets the content of the comment.
    /// </summary>
    public string Content { get; set; } = null!;

    /// <summary>
    /// Gets or sets the date and time when the comment was created.
    /// </summary>
    public DateTime CreatedAt { get; set; }
}
