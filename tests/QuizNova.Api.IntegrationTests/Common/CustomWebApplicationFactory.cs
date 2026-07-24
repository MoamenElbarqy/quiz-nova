using System.Data.Common;

using Microsoft.AspNetCore.Mvc.Testing;

using Testcontainers.MongoDb;
using Testcontainers.PostgreSql;

using Xunit;

namespace QuizNova.Api.IntegrationTests.Common;

public class CustomWebApplicationFactory : WebApplicationFactory<AssemblyMarker>, IAsyncLifetime
{
    private static readonly PostgreSqlContainer DbContainer = new PostgreSqlBuilder()
        .WithImage("postgres:18.3")
        .WithDatabase("postgres")
        .WithUsername("test_user")
        .WithPassword("test_password")
        .Build();

    private static readonly MongoDbContainer MongoContainer = new MongoDbBuilder()
        .WithImage("mongo:7.0")
        .Build();

    // for thread safety because we run xUnit in parallel mode
    private static readonly Lazy<Task> StartLazy = new(async () =>
    {
        await Task.WhenAll(DbContainer.StartAsync(), MongoContainer.StartAsync());
    });

    private string? _connectionString;
    private string? _mongoDatabaseName;

    public async Task InitializeAsync()
    {
        await StartLazy.Value;

        // separate database for each test run
        var dbName = $"quiznova_test_{Guid.NewGuid():N}";
        _mongoDatabaseName = $"quiznova_mongo_test_{Guid.NewGuid():N}";

        var baseConnectionString = DbContainer.GetConnectionString();
        var connBuilder = new DbConnectionStringBuilder
        {
            ConnectionString = baseConnectionString,
            ["Database"] = dbName,
            ["Maximum Pool Size"] = 5,
            ["Minimum Pool Size"] = 0,
        };
        _connectionString = connBuilder.ConnectionString;
    }

    public new Task DisposeAsync()
    {
        // We do not stop the container here because it is a shared singleton.
        // Ryuk will clean up the container when the test process finishes.
        return Task.CompletedTask;
    }

    public AppHttpClient CreateAppHttpClient()
    {
        return new AppHttpClient(CreateManualClient());
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.UseSetting("ConnectionStrings:DefaultConnection", _connectionString);
        builder.UseSetting("MongoDbSettings:ConnectionString", MongoContainer.GetConnectionString());
        builder.UseSetting("MongoDbSettings:DatabaseName", _mongoDatabaseName);
        builder.UseSetting("AutoMigrateDb", "true");
        builder.UseSetting("DisableRateLimiting", "true");
        builder.UseSetting("JwtSettings:Secret", "QuizNova-Development-Secret-Key-Change-This-2026-Super-Long-Key");
        builder.UseSetting("JwtSettings:Issuer", "QuizNova.Api");
        builder.UseSetting("JwtSettings:Audiences:0", "QuizNova.Client");
    }

    private HttpClient CreateManualClient()
    {
        return CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false, // disable the client from following the redirect response
            HandleCookies = false, // disable the client from sending the cookie header every time when we set
        });
    }
}
