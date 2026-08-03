using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.RateLimiting;

using Asp.Versioning;

using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.Options;

using Npgsql;

using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

using QuizNova.Api.Infrastructure;
using QuizNova.Api.OpenApi.Transformers;
using QuizNova.Api.services;
using QuizNova.Application.Common.Caching;
using QuizNova.Application.Common.Interfaces;
using QuizNova.Infrastructure.Settings;

namespace QuizNova.Api;

public static class DependencyInjection
{
    public static IServiceCollection AddApi(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddControllerWithJsonConfiguration();
        services.Configure<ForwardedHeadersOptions>(options =>
        {
            options.ForwardedHeaders = ForwardedHeaders.XForwardedProto | ForwardedHeaders.XForwardedFor;
            options.KnownIPNetworks.Clear();
            options.KnownProxies.Clear();
        });
        services.AddCustomVersioning();
        services.AddApiDocumentation();
        services.AddAppOpenTelemetry();
        services.AddGracefulShutdown();

        services.AddOutputCache();
        services.AddOptions<OutputCacheOptions>()
            .Configure<IOptions<CacheSettings>>((options, cacheSettings) =>
            {
                options.AddBasePolicy(builder =>
                    builder.Expire(TimeSpan.FromSeconds(cacheSettings.Value.OutputCacheDurationSeconds)));
            });

        services.AddRateLimiter();
        services.AddOptions<RateLimiterOptions>()
            .Configure<IOptions<RateLimiterSettings>>((options, rateLimiterSettings) =>
            {
                options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

                var settings = rateLimiterSettings.Value;

                if (settings.DisableRateLimiting)
                {
                    options.AddPolicy(RateLimiterPolicies.Global, _ => RateLimitPartition.GetNoLimiter("global"));
                    options.AddPolicy(RateLimiterPolicies.SubmitQuiz, _ => RateLimitPartition.GetNoLimiter("submitquiz"));
                    options.AddPolicy(RateLimiterPolicies.Auth, _ => RateLimitPartition.GetNoLimiter("auth"));
                    return;
                }

                options.AddConcurrencyLimiter(RateLimiterPolicies.Global, limiter =>
                {
                    limiter.PermitLimit = settings.GlobalConcurrencyPermitLimit;
                    limiter.QueueLimit = settings.GlobalConcurrencyQueueLimit;
                    limiter.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                });

                options.AddTokenBucketLimiter(RateLimiterPolicies.SubmitQuiz, limiter =>
                {
                    limiter.TokenLimit = settings.SubmitQuizTokenLimit;
                    limiter.ReplenishmentPeriod = TimeSpan.FromSeconds(settings.SubmitQuizReplenishmentPeriodSeconds);
                    limiter.TokensPerPeriod = settings.SubmitQuizTokensPerPeriod;
                    limiter.QueueLimit = settings.SubmitQuizQueueLimit;
                    limiter.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                    limiter.AutoReplenishment = true;
                });

                options.AddPolicy(RateLimiterPolicies.Auth, ctx =>
                    RateLimitPartition.GetSlidingWindowLimiter(
                        ctx.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                        _ => new SlidingWindowRateLimiterOptions
                        {
                            Window = TimeSpan.FromMinutes(settings.AuthWindowMinutes),
                            PermitLimit = settings.AuthPermitLimit,
                            SegmentsPerWindow = settings.AuthSegmentsPerWindow,
                            QueueLimit = 0,
                        }));
            });

        services.AddConfiguredCors();
        services.AddExceptionHandling();
        services.AddProblemDetails();
        services.AddAuthorization();

        services.AddSignalR();
        services.AddCustomResponseCompression();
        services.AddIdentityInfrastructure();

        services.AddOptions<PostgresSettings>()
            .Bind(configuration.GetSection(PostgresSettings.SectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddOptions<MongoDbSettings>()
            .Bind(configuration.GetSection(MongoDbSettings.SectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        return services;
    }

    private static IServiceCollection AddGracefulShutdown(this IServiceCollection services)
    {
        services.Configure<HostOptions>(options =>
        {
            options.ShutdownTimeout = TimeSpan.FromSeconds(30);
        });
        return services;
    }

    private static IServiceCollection AddIdentityInfrastructure(this IServiceCollection services)
    {
        services.AddScoped<IUser, CurrentUser>();
        services.AddHttpContextAccessor();
        return services;
    }

    private static IServiceCollection AddCustomResponseCompression(this IServiceCollection services)
    {
        services.AddResponseCompression(options =>
        {
            options.EnableForHttps = true;
            options.Providers.Add<BrotliCompressionProvider>();
            options.Providers.Add<GzipCompressionProvider>();
        });
        return services;
    }

    private static IServiceCollection AddCustomVersioning(
        this IServiceCollection
            services)
    {
        services
            .AddApiVersioning(options =>
            {
                options.DefaultApiVersion = new ApiVersion(1, 0);
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.ReportApiVersions = true;
            })
            .AddApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'VVV";
                options.SubstituteApiVersionInUrl = true;
            });
        return services;
    }

    private static IServiceCollection AddApiDocumentation(this IServiceCollection services)
    {
        string[] versions = ["v1"];

        foreach (var version in versions)
        {
            services.AddOpenApi(
                version,
                options =>
                {
                    // Versioning config
                    options.AddDocumentTransformer<VersionInfoTransformer>();

                    // Security Scheme config
                    options.AddDocumentTransformer<BearerSecuritySchemeTransformer>();

                    // Security Operation config
                    options.AddOperationTransformer<BearerSecurityOperationTransformer>();
                });
        }

        return services;
    }

    private static IServiceCollection AddControllerWithJsonConfiguration(this IServiceCollection services)
    {
        services.AddControllers().AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
            options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            options.JsonSerializerOptions.AllowOutOfOrderMetadataProperties = true;
        });

        services.ConfigureHttpJsonOptions(options =>
        {
            options.SerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
            options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            options.SerializerOptions.AllowOutOfOrderMetadataProperties = true;
        });

        return services;
    }

    private static IServiceCollection AddConfiguredCors(this IServiceCollection services)
    {
        services.AddCors();
        services.AddOptions<CorsOptions>()
            .Configure<IOptions<CorsSettings>>((options, corsSettings) =>
            {
                options.AddPolicy(
                    corsSettings.Value.PolicyName,
                    policy =>
                    {
                        policy.WithOrigins(corsSettings.Value.AllowedOrigins)
                            .AllowAnyMethod()
                            .AllowAnyHeader()
                            .AllowCredentials();
                    });
            });

        return services;
    }

    private static IServiceCollection AddExceptionHandling(this IServiceCollection services)
    {
        services.AddExceptionHandler<GlobalExceptionHandler>();
        return services;
    }

    private static IServiceCollection AddProblemDetails(this IServiceCollection services)
    {
        services.AddProblemDetails(options => options.CustomizeProblemDetails = (context) =>
        {
            // add the request path for example GET /quizzes/123
            context.ProblemDetails.Instance =
                $"{context.HttpContext.Request.Method} {context.HttpContext.Request.Path}";

            // we put the trace id in case the client report and error we can trace what is the error in the logs
            context.ProblemDetails.Extensions.Add("requestId", context.HttpContext.TraceIdentifier);
        });
        return services;
    }

    private static IServiceCollection AddAppOpenTelemetry(this IServiceCollection services)
    {
        services.AddOpenTelemetry()
            .ConfigureResource(res => res.AddService("api"))
            .WithTracing(tracing =>
            {
                // trace the incoming and outgoing HTTP requests
                tracing.AddAspNetCoreInstrumentation().AddHttpClientInstrumentation();

                // trace PostgreSQL and MongoDB database queries
                tracing.AddNpgsql()
                       .AddSource("MongoDB.Driver.Core.Extensions.DiagnosticSources");

                // export the traces to the endpoint defined in OTEL_EXPORTER_OTLP_ENDPOINT defined in compose that will go to the seq
                tracing.AddOtlpExporter();
            }).WithMetrics(metrics =>
            {
                metrics.AddAspNetCoreInstrumentation().AddHttpClientInstrumentation();

                // export metrics via OTLP
                metrics.AddOtlpExporter();
            });

        return services;
    }
}
