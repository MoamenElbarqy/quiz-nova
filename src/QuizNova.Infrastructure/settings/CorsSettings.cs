namespace QuizNova.Infrastructure.Settings;

public sealed class CorsSettings
{
    public const string SectionName = "CorsSettings";

    public required string PolicyName { get; init; }

    public string[] AllowedOrigins { get; init; } = [];
}
