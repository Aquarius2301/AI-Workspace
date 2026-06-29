using BusinessObject.Enums;

namespace BusinessObject.Entities;

public class Attachment
{
    /// <summary>
    /// Gets or sets the unique identifier of the attachment.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the type of entity this attachment references.
    /// </summary>
    public ReferenceType ReferenceType { get; set; } = ReferenceType.Document;

    /// <summary>
    /// Gets or sets the unique identifier of the referenced entity.
    /// </summary>
    public Guid ReferenceId { get; set; }

    /// <summary>
    /// Gets or sets the file name of the attachment.
    /// </summary>
    public string FileName { get; set; } = null!;

    /// <summary>
    /// Gets or sets the optional content type of the attachment.
    /// </summary>
    public string? ContentType { get; set; }

    /// <summary>
    /// Gets or sets the URL of the attachment file.
    /// </summary>
    public string Url { get; set; } = null!;

    /// <summary>
    /// Gets or sets the date and time when the attachment was uploaded.
    /// </summary>
    public DateTimeOffset UploadedAt { get; set; }
}
