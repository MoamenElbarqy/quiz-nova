namespace QuizNova.Infrastructure.Settings;

public sealed class MongoDbSettings
{
    public const string SectionName = "MongoDbSettings";

    public string ConnectionString { get; init; }
    public string DatabaseName { get; init; }
}
