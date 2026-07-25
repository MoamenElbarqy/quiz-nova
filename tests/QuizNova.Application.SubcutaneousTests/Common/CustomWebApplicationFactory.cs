using System.Data.Common;

using MediatR;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Time.Testing;

using QuizNova.Api;
using QuizNova.Application.Common.Interfaces;
using QuizNova.Tests.Common.Security;

using Testcontainers.MongoDb;
using Testcontainers.PostgreSql;

namespace QuizNova.Application.SubcutaneousTests.Common;

public class CustomWebApplicationFactory : WebApplicationFactory<AssemblyMarker>, IAsyncLifetime
{
    static CustomWebApplicationFactory()
    {
        Environment.SetEnvironmentVariable("DOTNET_USE_POLLING_FILE_WATCHER", "1");
        Environment.SetEnvironmentVariable("ASPNETCORE_hostBuilder_reloadConfigOnChange", "false");
        Environment.SetEnvironmentVariable("DOTNET_hostBuilder_reloadConfigOnChange", "false");
    }

    private static readonly PostgreSqlContainer DbContainer = new PostgreSqlBuilder()
        .WithImage("postgres:18.3")
        .WithDatabase("postgres")
        .WithUsername("test_user")
        .WithPassword("test_password")
        .WithCommand("-c", "max_connections=500")
        .Build();

    private static readonly MongoDbContainer MongoContainer = new MongoDbBuilder()
        .WithImage("mongo:7.0")
        .Build();

    // for thread safety because we run xUnit in parallel mode
    private static readonly Lazy<Task> StartLazy = new(async () =>
    {
        await Task.WhenAll(DbContainer.StartAsync(), MongoContainer.StartAsync());
    });

    private readonly FakeTimeProvider _fakeTimeProvider = new(DateTimeOffset.UtcNow);

    private string? _connectionString;
    private string? _mongoDatabaseName;

    public FakeTimeProvider GetFakeTimeProvider() => _fakeTimeProvider;

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
            ["Maximum Pool Size"] = 2,
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

    public IMediator CreateMediator()
    {
        var serviceScope = Services.CreateScope();

        return serviceScope.ServiceProvider.GetRequiredService<IMediator>();
    }

    public AppHttpClient CreateAppHttpClient()
    {
        return new AppHttpClient(CreateManualClient());
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureAppConfiguration((_, configBuilder) =>
        {
            foreach (var source in configBuilder.Sources.OfType<FileConfigurationSource>())
            {
                source.ReloadOnChange = false;
            }
        });

        builder.ConfigureServices(services =>
        {
            var userDescriptors = services.Where(d => d.ServiceType == typeof(IUser)).ToList();
            foreach (var d in userDescriptors)
            {
                services.Remove(d);
            }

            services.AddScoped<IUser, TestCurrentUser>();

            // Replace TimeProvider.System with FakeTimeProvider for testable clock
            var timeProviderDescriptors = services.Where(d => d.ServiceType == typeof(TimeProvider)).ToList();
            foreach (var d in timeProviderDescriptors)
            {
                services.Remove(d);
            }

            services.AddSingleton<TimeProvider>(_fakeTimeProvider);
        });

        builder.UseSetting("PostgresSettings:DefaultConnection", _connectionString);
        builder.UseSetting("MongoDbSettings:ConnectionString", MongoContainer.GetConnectionString());
        builder.UseSetting("MongoDbSettings:DatabaseName", _mongoDatabaseName);
        builder.UseSetting("JwtSettings:Secret", "QuizNova-Development-Secret-Key-Change-This-2026-Super-Long-Key");
        builder.UseSetting("JwtSettings:Issuer", "QuizNova.Api");
        builder.UseSetting("JwtSettings:Audiences:0", "QuizNova.Client");
        builder.UseSetting("JwtSettings:ExpiryMinutes", "7");
        builder.UseSetting("JwtSettings:RefreshTokenExpirationDays", "7");
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
