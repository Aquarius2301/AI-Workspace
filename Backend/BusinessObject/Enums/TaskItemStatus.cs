namespace BusinessObject.Enums;

public enum TaskItemStatus
{
    /// <summary>
    ///  The task is in the open state, meaning it has been created but work has not yet started on it.
    /// </summary>
    Open,

    /// <summary>
    /// The task is currently in progress, meaning work has started on it but it has not yet been completed.
    /// </summary>
    InProgress,

    /// <summary>
    /// The task has been completed, meaning all work on it has been finished and it is considered done.
    /// </summary>
    Done,

    /// <summary>
    /// The task is on hold, meaning work on it has been temporarily paused and it is not currently being worked on.
    /// </summary>
    Blocked,
}
