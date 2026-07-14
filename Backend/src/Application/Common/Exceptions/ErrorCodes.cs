namespace AIWorkspace.Application.Common.Exceptions;

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
    public const string TooManyRequests = "TooManyRequests";

    // ─── Auth ────────────────────────────────────────────────
    public const string LoginFailed = "LoginFailed";
    public const string EmailRequired = "EmailRequired";
    public const string PasswordRequired = "PasswordRequired";
    public const string NameRequired = "NameRequired";
    public const string NameMaxLength = "NameMaxLength";
    public const string EmailInvalid = "EmailInvalid";
    public const string PasswordInvalid = "PasswordInvalid";
    public const string EmailAlreadyExists = "EmailAlreadyExists";
    public const string RefreshTokenRequired = "RefreshTokenRequired";
    public const string DeviceIdRequired = "DeviceIdRequired";
    public const string UserIdRequired = "UserIdRequired";

    // ─── Project ─────────────────────────────────────────────
    public const string ProjectNameRequired = "ProjectNameRequired";
    public const string ProjectNameMaxLength = "ProjectNameMaxLength";
    public const string ProjectNotFound = "ProjectNotFound";

    // ─── Team ────────────────────────────────────────────────
    public const string TeamNotFound = "TeamNotFound";
    public const string TeamNameRequired = "TeamNameRequired";
    public const string TeamNameMaxLength = "TeamNameMaxLength";
    public const string TeamDescriptionMaxLength = "TeamDescriptionMaxLength";

    // ─── Upload ──────────────────────────────────────────────
    public const string FileRequired = "FileRequired";
    public const string FileSizeTooLarge = "FileSizeTooLarge";
    public const string ImageKitUploadFailed = "ImageKitUploadFailed";

    // ─── User ──────────────────────────────────────────────
    public const string UserNotFound = "UserNotFound";
    public const string PictureNotFound = "PictureNotFound";
}
