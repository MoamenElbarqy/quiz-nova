using System.Diagnostics.CodeAnalysis;

namespace QuizNova.Infrastructure.Settings;

public class AppSettings
{
    public const string SectionName = "AppSettings";

    public CorsSettings Cors { get; init; } = new();
}

public class CorsSettings
{
    [SetsRequiredMembers]
    public CorsSettings()
    {
    }

    public required string PolicyName { get; init; }

    public string[] AllowedOrigins { get; init; } = [];
}
