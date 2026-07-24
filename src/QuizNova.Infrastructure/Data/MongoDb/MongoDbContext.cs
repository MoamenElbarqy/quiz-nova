namespace QuizNova.Infrastructure.Data.MongoDb;

using Application.Common.Interfaces;

using Domain.Entities.QuizAttempts;
using Domain.Entities.Quizzes;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

using MongoDB.Driver;
using MongoDB.Driver.Core.Extensions.DiagnosticSources;

using Settings;

public sealed class MongoDbContext : IMongoDbContext
{
    private readonly IMongoDatabase _database;

    public MongoDbContext(IOptions<MongoDbSettings> settings, IHostEnvironment environment)
    {
        MongoDbClassMapper.RegisterClassMaps();

        var clientSettings = MongoClientSettings.FromConnectionString(settings.Value.ConnectionString);

        if (!environment.IsEnvironment("Testing"))
        {
            clientSettings.ClusterConfigurator = cb => cb.Subscribe(new DiagnosticsActivityEventSubscriber());
        }

        var client = new MongoClient(clientSettings);
        _database = client.GetDatabase(settings.Value.DatabaseName);
    }

    public IMongoCollection<Quiz> Quizzes => _database.GetCollection<Quiz>("quizzes");

    public IMongoCollection<QuizAttempt> QuizAttempts => _database.GetCollection<QuizAttempt>("quiz_attempts");
}
