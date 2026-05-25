using MediatR;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Testcontainers.PostgreSql;

namespace QuizNova.Api.SubcutaneousTests.Common;

public class CustomWebApplicationFactory : WebApplicationFactory<AssemblyMarker>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _dbContainer = new PostgreSqlBuilder()
        .WithImage("postgres:18.3")
        .WithDatabase("quiznova_test")
        .WithUsername("test_user")
        .WithPassword("test_password")
        .Build();

    public IMediator CreateMediator()
    {
        var serviceScope = Services.CreateScope();

        return serviceScope.ServiceProvider.GetRequiredService<IMediator>();
    }

    public async Task InitializeAsync()
    {
        await _dbContainer.StartAsync();
        Environment.SetEnvironmentVariable("ConnectionStrings__DefaultConnection", _dbContainer.GetConnectionString());
        Environment.SetEnvironmentVariable("ConnectionStrings:DefaultConnection", _dbContainer.GetConnectionString());
        Environment.SetEnvironmentVariable("AutoMigrateDb", "true");
    }

    public new async Task DisposeAsync()
    {
        await _dbContainer.StopAsync();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration((_, configBuilder) =>
        {
            configBuilder.AddInMemoryCollection(new Dictionary<string, string?>
            {
                { "ConnectionStrings:DefaultConnection", _dbContainer.GetConnectionString() },
                { "AutoMigrateDb", "true" },
                { "JwtSettings:Secret", "QuizNova-Development-Secret-Key-Change-This-2026-Super-Long-Key" },
                { "JwtSettings:Issuer", "QuizNova.Api" },
                { "JwtSettings:Audience", "QuizNova.Client" },
            });
        });
    }
}
