using System.ComponentModel.DataAnnotations;

namespace QuizNova.Infrastructure.Settings;

public sealed class CorsSettings
{
    public const string SectionName = "CorsSettings";

    [Required]
    public required string PolicyName { get; init; }

    public string[] AllowedOrigins { get; init; } = [];
}
