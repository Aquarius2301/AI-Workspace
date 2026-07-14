namespace AIWorkspace.Domain.Enums;

public enum ProjectVisibility
{
    /// <summary>
    /// Only team members with access to the project can see and access it
    /// </summary>
    Private,

    /// <summary>
    /// Anyone in a team can see and access this project
    /// </summary>
    Public,
}
