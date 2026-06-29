namespace BusinessObject.Entities;

public class Document
{
    /// <summary>
    /// Gets or sets the unique identifier of the document.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the unique identifier of the project that owns the document.
    /// </summary>
    public Guid ProjectId { get; set; }

    /// <summary>
    /// Gets or sets the project that owns this document.
    /// </summary>
    public Project Project { get; set; } = null!;

    /// <summary>
    /// Gets or sets the user ID of the creator of the document.
    /// </summary>
    public Guid CreatorId { get; set; }

    /// <summary>
    /// Gets or sets the user who created the document.
    /// </summary>
    public User Creator { get; set; } = null!;

    /// <summary>
    /// Gets or sets the title of the document.
    /// </summary>
    public string Title { get; set; } = null!;

    /// <summary>
    /// Gets or sets the optional content of the document in markdown or rich text format.
    /// </summary>
    public string? Content { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the document was created.
    /// </summary>
    public DateTimeOffset CreatedAt { get; set; }
}
