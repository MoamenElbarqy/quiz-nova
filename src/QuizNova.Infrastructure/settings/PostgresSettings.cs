using System.ComponentModel.DataAnnotations;

namespace QuizNova.Infrastructure.Settings;

public sealed class PostgresSettings
{
    public const string SectionName = "PostgresSettings";

    [Required]
    public required string DefaultConnection { get; init; }
}
