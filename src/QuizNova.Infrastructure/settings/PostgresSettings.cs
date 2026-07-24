namespace QuizNova.Infrastructure.Settings;

public sealed class PostgresSettings
{
    public const string SectionName = "ConnectionStrings";

    public required string DefaultConnection { get; init; }
}
