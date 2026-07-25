using System.ComponentModel.DataAnnotations;

namespace QuizNova.Infrastructure.Settings;

public sealed class MongoDbSettings
{
    public const string SectionName = "MongoDbSettings";

    [Required]
    public required string ConnectionString { get; init; }

    [Required]
    public required string DatabaseName { get; init; }
}
