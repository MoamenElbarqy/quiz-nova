namespace QuizNova.Infrastructure.Settings;

public class AppSettings
{
    public const string SectionName = "AppSettings";

    public CorsSettings Cors { get; init; } = new();
}

public class CorsSettings
{
    public string PolicyName { get; set; } = string.Empty;

    public string[] AllowedOrigins { get; set; } = Array.Empty<string>();
}
