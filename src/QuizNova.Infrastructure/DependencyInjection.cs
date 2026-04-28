using System.Text;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

using QuizNova.Application.Common.Interfaces;
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
        services.Configure<AppSettings>(configuration.GetSection(AppSettings.SectionName));
        services.Configure<JwtSettings>(configuration.GetSection(JwtSettings.SectionName));
        return services;
    }

    private static IServiceCollection AddJwtAuthentication(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var jwtSection = configuration.GetSection(JwtSettings.SectionName);

        var jwtSettings = jwtSection.Get<JwtSettings>()
                          ?? throw new InvalidOperationException("JwtSettings configuration is missing.");

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtSettings.Issuer,
                ValidAudience = jwtSettings.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Secret)),
            };
        });
        return services;
    }
}
