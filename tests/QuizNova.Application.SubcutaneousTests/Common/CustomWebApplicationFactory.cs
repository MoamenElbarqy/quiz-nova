using System.Data.Common;

using MediatR;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using QuizNova.Api;
using QuizNova.Application.Common.Interfaces;
using QuizNova.Tests.Common.Security;

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
        .Build();

    // for thread safty due we run the xunit in parallel mode so multiple tests start
    // initializing the factory at the same time, so we need one container with multiple databases.
    private static readonly Lazy<Task> StartLazy = new(async () =>
    {
        await DbContainer.StartAsync();
    });

    private string? _connectionString;

    public IMediator CreateMediator()
    {
        var serviceScope = Services.CreateScope();

        return serviceScope.ServiceProvider.GetRequiredService<IMediator>();
    }

    public async Task InitializeAsync()
    {
        await StartLazy.Value;

        // seprate database for each test run
        var dbName = $"quiznova_test_{Guid.NewGuid():N}";

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

    public HttpClient CreateManualClient()
    {
        return CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false, // disable the client from following the redirect response
            HandleCookies = false, // disable the client from sending the cookie header every time when we set
        });
    }

    public AppHttpClient CreateAppHttpClient()
    {
        return new AppHttpClient(CreateManualClient());
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration((_, configBuilder) =>
        {
            foreach (var source in configBuilder.Sources.OfType<FileConfigurationSource>())
            {
                source.ReloadOnChange = false;
            }
        });

        builder.ConfigureServices(services =>
        {
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(IUser));
            if (descriptor != null)
            {
                services.Remove(descriptor);
            }

            services.AddScoped<IUser, TestCurrentUser>();
        });

        builder.UseSetting("ConnectionStrings:DefaultConnection", _connectionString);
        builder.UseSetting("AutoMigrateDb", "true");
        builder.UseSetting("JwtSettings:Secret", "QuizNova-Development-Secret-Key-Change-This-2026-Super-Long-Key");
        builder.UseSetting("JwtSettings:Issuer", "QuizNova.Api");
        builder.UseSetting("JwtSettings:Audience", "QuizNova.Client");
    }
}
