namespace AIWorkspace.Domain.Enums;

public enum TaskItemStatus
{
    /// <summary>
    /// The task is pending, meaning it has been created but work has not yet started on it.
    /// </summary>
    ToDo,

    /// <summary>
    /// The task is currently in progress, meaning work has started on it but it has not yet been completed.
    /// </summary>
    Doing,

    /// <summary>
    /// The task has been completed, meaning all work on it has been finished and it is considered done.
    /// </summary>
    Done,
}
