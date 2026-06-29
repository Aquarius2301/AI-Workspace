namespace BusinessObject.Entities;

public class AiInteraction
{
    /// <summary>
    /// Gets or sets the unique identifier of the AI interaction.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the optional unique identifier of the user who created the interaction.
    /// </summary>
    public Guid? UserId { get; set; }

    /// <summary>
    /// Gets or sets the optional user who created the interaction.
    /// </summary>
    public User? User { get; set; }

    /// <summary>
    /// Gets or sets the optional unique identifier of the related project.
    /// </summary>
    public Guid? ProjectId { get; set; }

    /// <summary>
    /// Gets or sets the optional project related to the interaction.
    /// </summary>
    public Project? Project { get; set; }

    /// <summary>
    /// Gets or sets the name of the AI model used for the interaction.
    /// </summary>
    public string Model { get; set; } = null!;

    /// <summary>
    /// Gets or sets the prompt sent to the AI model.
    /// </summary>
    public string Prompt { get; set; } = null!;

    /// <summary>
    /// Gets or sets the response text returned by the AI model.
    /// </summary>
    public string ResponseText { get; set; } = null!;

    /// <summary>
    /// Gets or sets the date and time when the AI interaction was created.
    /// </summary>
    public DateTimeOffset CreatedAt { get; set; }
}
