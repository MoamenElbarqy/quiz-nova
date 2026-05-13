using System.Text;

using Community.Microsoft.Extensions.Caching.PostgreSql;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

using QuizNova.Application.Common.Interfaces;
using QuizNova.Infrastructure.Caching;
using QuizNova.Infrastructure.Data;
using QuizNova.Infrastructure.Services;
using QuizNova.Infrastructure.Settings;

namespace Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.ConfigureSettings(configuration);
        services.AddJwtAuthentication(configuration);
        services.ConfigureDataBase(configuration);
        services.ConfigureCaching(configuration);
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<DbInitializer>();

        return services;
    }

    public static async Task InitializeDevelopmentDatabaseAsync(
        this IServiceProvider serviceProvider,
        CancellationToken ct = default)
    {
        using var scope = serviceProvider.CreateScope();
        var dbInitializer = scope.ServiceProvider.GetRequiredService<DbInitializer>();
        await dbInitializer.InitializeAsync(ct);
    }

    private static IServiceCollection ConfigureCaching(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
                               ?? throw new InvalidOperationException(
                                   "The connection string 'DefaultConnection' is not configured.");

        services.AddDistributedPostgreSqlCache(options =>
        {
            options.ConnectionString = connectionString;
            options.SchemaName = "public";
            options.TableName = "Cache";
            options.CreateInfrastructure = true;
        });

        services.AddHybridCache();

        services.AddScoped<ICacheInvalidator, CacheInvalidator>();

        return services;
    }

    private static IServiceCollection ConfigureDataBase(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException("The connection string 'DefaultConnection' is not configured.");
        }

        services.AddDbContext<AppDbContext>(options => options.UseNpgsql(connectionString));

        services.AddScoped<IAppDbContext>(provider => provider.GetRequiredService<AppDbContext>());

        return services;
    }

    private static IServiceCollection ConfigureSettings(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<AppSettings>()
            .Bind(configuration.GetSection(AppSettings.SectionName));

        services.AddOptions<JwtSettings>()
            .Bind(configuration.GetSection(JwtSettings.SectionName));

        return services;
    }

    private static IServiceCollection AddJwtAuthentication(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer();

        services.AddOptions<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme)
            .Configure<IOptions<JwtSettings>>((options, jwtSettings) =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings.Value.Issuer,
                    ValidAudience = jwtSettings.Value.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(jwtSettings.Value.Secret)),
                };
            });

        return services;
    }
}
