namespace AIWorkspace.Domain.Entities;

public class Picture
{
    /// <summary>
    /// Gets or sets the unique identifier of the picture.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the ImageKit file ID used for deletion.
    /// </summary>
    public string FileId { get; set; } = null!;

    /// <summary>
    /// Gets or sets the CDN URL of the picture.
    /// </summary>
    public string Url { get; set; } = null!;

    /// <summary>
    /// Gets or sets whether this picture is currently in use by a user.
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the picture was created.
    /// </summary>
    public DateTimeOffset CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the user currently using this picture as avatar (optional).
    /// </summary>
    public Guid? UsedByUserId { get; set; }

    /// <summary>
    /// Gets or sets the user currently using this picture as avatar.
    /// </summary>
    public User? UsedByUser { get; set; }
}
