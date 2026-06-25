namespace BusinessObject.Enums;

/// <summary>
/// Defines the role and access control level of a user within a specific team.
/// </summary>
public enum TeamMemberRole
{
    /// The team administrator.
    /// Has highest privileges, including managing team settings, inviting/removing members,
    /// assigning roles, and performing full CRUD operations on all team resources.
    Admin,

    /// The project or technical leader.
    /// Responsible for work management, including creating/modifying projects,
    /// managing tasks, handling project documents, and configuring AI workspace contexts.
    Leader,

    /// A regular team member.
    /// Focused on execution, including viewing assigned tasks, updating task statuses,
    /// writing comments, managing attachments, and interacting with the AI companion.
    Member,
}
