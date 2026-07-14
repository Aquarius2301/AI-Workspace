namespace AIWorkspace.Infrastructure.Settings;

public class UploadSetting
{
    public const string SectionName = "UploadSetting";

    public string PublicKey { get; set; } = string.Empty;
    public string PrivateKey { get; set; } = string.Empty;
    public string UrlEndpoint { get; set; } = string.Empty;
}
