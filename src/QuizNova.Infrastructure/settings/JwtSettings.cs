namespace QuizNova.Infrastructure.Settings;

public class JwtSettings
{
    public const string SectionName = "JwtSettings";

    public required string Issuer { get; init; }

    public required string Audience { get; init; }

    public required string Secret { get; init; }

    public int ExpiryMinutes { get; init; }
}
