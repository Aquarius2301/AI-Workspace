namespace AIWorkspace.Infrastructure.Settings;

public class RateLimitSetting
{
    public const string SectionName = "RateLimitSetting";

    public int PermitLimit { get; set; } = 5;
    public int WindowMinutes { get; set; } = 1;
}
