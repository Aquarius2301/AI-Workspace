namespace Infrastructure.Exceptions
{
    /// <summary>
    /// Centralized error codes for API responses.
    /// Frontend uses these codes for i18n / multi-language support.
    /// Convention: PascalCase, no spaces, descriptive of the error.
    /// </summary>
    public static class ErrorCodes
    {
        // ─── Common ────────────────────────────────────────────────
        public const string InternalServerError = "InternalServerError";
        public const string BadRequest = "BadRequest";
        public const string Unauthorized = "Unauthorized";
        public const string Forbidden = "Forbidden";
        public const string Conflict = "Conflict";
        public const string NotFound = "NotFound";

        // ─── Auth ────────────────────────────────────────────────
        public const string EmailPasswordRequired = "EmailPasswordRequired";
        public const string EmailAlreadyExists = "EmailAlreadyExists";
        public const string InvalidEmailOrPassword = "InvalidEmailOrPassword";
        public const string InvalidRefreshToken = "InvalidRefreshToken";
        public const string UserNotFound = "UserNotFound";
        public const string NameRequired = "NameRequired";
        public const string EmailRequired = "EmailRequired";
        public const string PasswordRequired = "PasswordRequired";

        // ─── Password Validation ─────────────────────────────────
        public const string PasswordInvalid = "PasswordInvalid";
        public const string PasswordRequireUpper = "PasswordRequireUpper";
        public const string PasswordRequireSpecial = "PasswordRequireSpecial";
        public const string OldPasswordIncorrect = "OldPasswordIncorrect";

        // ─── NotFound ───────────────────────────────────────────
        public const string MemberNotFound = "MemberNotFound";
        public const string TeamNotFound = "TeamNotFound";

        // ─── Member / Team ──────────────────────────────────────
        public const string NotTeamMember = "NotTeamMember";
        public const string TeamMinOneAdmin = "TeamMinOneAdmin";
        public const string TeamMinOneAdminTransferRole = "TeamMinOneAdminTransferRole";
        public const string InvalidRoleRequest = "InvalidRoleRequest";
        public const string TeamNameRequired = "TeamNameRequired";
        public const string OneMemberRequired = "OneMemberRequired";

        // ─── Permission (Forbidden) ─────────────────────────────
        public const string NoPermissionRemoveTeamMember = "NoPermissionRemoveTeamMember";
        public const string NoPermissionUpdateMemberRole = "NoPermissionUpdateMemberRole";
        public const string NoPermissionAddMemberRole = "NoPermissionAddMemberRole";
        public const string NoPermissionManageTeam = "NoPermissionManageTeam";

        public const string NoPermissionChangePassword = "NoPermissionChangePassword";

        // ─── Role-based (dynamic) ───────────────────────────────
        // Usage: throw new ForbiddenException(string.Format(RequireTeamRole, roleList));
        public const string RequireTeamRole = "RequireTeamRole:{0}";
    }
}
