using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;

using MongoDB.Driver;

using QuizNova.Infrastructure.Settings;

namespace QuizNova.Api.Infrastructure;

public sealed class MongoHealthCheck : IHealthCheck
{
    private readonly MongoDbSettings _settings;

    public MongoHealthCheck(IOptions<MongoDbSettings> settings)
    {
        _settings = settings.Value;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var client = new MongoClient(_settings.ConnectionString);
            var database = client.GetDatabase(_settings.DatabaseName);
            await database.RunCommandAsync<MongoDB.Bson.BsonDocument>(
                new MongoDB.Bson.BsonDocument("ping", 1),
                cancellationToken: cancellationToken);

            return HealthCheckResult.Healthy();
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("MongoDB is not reachable", ex);
        }
    }
}
