namespace Infrastructure.Exceptions
{
    /// <summary>
    /// Centralized error codes for API responses.
    /// Frontend uses these codes for i18n / multi-language support.
    /// Convention: PascalCase, no spaces, descriptive of the error.
    /// </summary>
    public static class ErrorCodes
    {
        // ─── Auth ────────────────────────────────────────────────
        public const string EmailAlreadyExists = "EmailAlreadyExists";
        public const string InvalidEmailOrPassword = "InvalidEmailOrPassword";
        public const string InvalidRefreshToken = "InvalidRefreshToken";
        public const string UserNotFound = "UserNotFound";

        // ─── Password Validation ─────────────────────────────────
        public const string PasswordMinLength = "PasswordMinLength";
        public const string PasswordRequireUpper = "PasswordRequireUpper";
        public const string PasswordRequireSpecial = "PasswordRequireSpecial";
        public const string OldPasswordIncorrect = "OldPasswordIncorrect";

        // ─── NotFound ───────────────────────────────────────────
        public const string CommentNotFound = "CommentNotFound";
        public const string DocumentNotFound = "DocumentNotFound";
        public const string MemberNotFound = "MemberNotFound";
        public const string ProjectMemberNotFound = "ProjectMemberNotFound";
        public const string ProjectNotFound = "ProjectNotFound";
        public const string ReferencedDocumentNotFound = "ReferencedDocumentNotFound";
        public const string ReferencedTaskNotFound = "ReferencedTaskNotFound";
        public const string TaskNotFound = "TaskNotFound";
        public const string TeamNotFound = "TeamNotFound";

        // ─── Member / Team ──────────────────────────────────────
        public const string NotTeamMember = "NotTeamMember";
        public const string UserNotTeamMember = "UserNotTeamMember";
        public const string AssignedUserNotTeamMember = "AssignedUserNotTeamMember";
        public const string UserAlreadyProjectMember = "UserAlreadyProjectMember";
        public const string NotPrivateProjectMember = "NotPrivateProjectMember";
        public const string TeamMinOneAdmin = "TeamMinOneAdmin";
        public const string TeamMinOneAdminTransferRole = "TeamMinOneAdminTransferRole";
        public const string InvalidRoleRequest = "InvalidRoleRequest";

        // ─── Comment ────────────────────────────────────────────
        public const string CommentContentRequired = "CommentContentRequired";
        public const string OwnCommentOnly = "OwnCommentOnly";

        // ─── Permission (Forbidden) ─────────────────────────────
        public const string AdminOnlyViewMembers = "AdminOnlyViewMembers";
        public const string NoPermissionCreateDocument = "NoPermissionCreateDocument";
        public const string NoPermissionDeleteComment = "NoPermissionDeleteComment";
        public const string NoPermissionDeleteDocument = "NoPermissionDeleteDocument";
        public const string NoPermissionDeleteProject = "NoPermissionDeleteProject";
        public const string NoPermissionDeleteTask = "NoPermissionDeleteTask";
        public const string NoPermissionAddProjectMember = "NoPermissionAddProjectMember";
        public const string NoPermissionAssignTask = "NoPermissionAssignTask";
        public const string NoPermissionChangeTaskStatus = "NoPermissionChangeTaskStatus";
        public const string NoPermissionCreateTask = "NoPermissionCreateTask";
        public const string NoPermissionRemoveProjectMember = "NoPermissionRemoveProjectMember";
        public const string NoPermissionUpdateDocument = "NoPermissionUpdateDocument";
        public const string NoPermissionUpdateProject = "NoPermissionUpdateProject";
        public const string NoPermissionUpdateTask = "NoPermissionUpdateTask";
        public const string NoPermissionViewProjectMembers = "NoPermissionViewProjectMembers";
        public const string NoPermissionDeleteTeam = "NoPermissionDeleteTeam";
        public const string NoPermissionLeaveTeam = "NoPermissionLeaveTeam";
        public const string NoPermissionUpdateTeam = "NoPermissionUpdateTeam";
        public const string NoPermissionRemoveTeamMember = "NoPermissionRemoveTeamMember";
        public const string NoPermissionUpdateMemberRole = "NoPermissionUpdateMemberRole";
        public const string NoPermissionManageTeam = "NoPermissionManageTeam";
        public const string NoPermissionViewTeamMembers = "NoPermissionViewTeamMembers";
        public const string NoPermissionCreateProject = "NoPermissionCreateProject";
        public const string NoPermissionViewAvailableMembers = "NoPermissionViewAvailableMembers";
        public const string NoPermissionUpdateProfile = "NoPermissionUpdateProfile";
        public const string NoPermissionChangePassword = "NoPermissionChangePassword";
        public const string NoPermissionUpdateActive = "NoPermissionUpdateActive";

        // ─── Role-based (dynamic) ───────────────────────────────
        // Usage: throw new ForbiddenException(string.Format(RequireTeamRole, roleList));
        public const string RequireTeamRole = "RequireTeamRole:{0}";
    }
}
