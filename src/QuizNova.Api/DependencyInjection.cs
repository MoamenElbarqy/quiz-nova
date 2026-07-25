using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.RateLimiting;

using Asp.Versioning;

using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.HttpOverrides;
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
        services.AddOutputCache(options =>
        {
            options.AddBasePolicy(builder =>
                builder.Expire(TimeSpan.FromSeconds(30)));
        });

        services.AddRateLimiter(options =>
        {
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

            if (configuration.GetValue<bool>("DisableRateLimiting"))
            {
                options.AddPolicy("Global", _ => RateLimitPartition.GetNoLimiter("global"));
                options.AddPolicy("SubmitQuiz", _ => RateLimitPartition.GetNoLimiter("submitquiz"));
                options.AddPolicy("Auth", _ => RateLimitPartition.GetNoLimiter("auth"));
                return;
            }

            options.AddConcurrencyLimiter("Global", limiter =>
            {
                limiter.PermitLimit = 50;
                limiter.QueueLimit = 100;
                limiter.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
            });

            options.AddTokenBucketLimiter("SubmitQuiz", limiter =>
            {
                limiter.TokenLimit = 200;
                limiter.ReplenishmentPeriod = TimeSpan.FromSeconds(1);
                limiter.TokensPerPeriod = 50;
                limiter.QueueLimit = 1000;
                limiter.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                limiter.AutoReplenishment = true;
            });

            options.AddPolicy("Auth", ctx =>
                RateLimitPartition.GetSlidingWindowLimiter(
                    ctx.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                    _ => new SlidingWindowRateLimiterOptions
                    {
                        Window = TimeSpan.FromMinutes(1),
                        PermitLimit = 5,
                        SegmentsPerWindow = 6,
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
            .Configure<IOptions<AppSettings>>((options, appSettings) =>
            {
                options.AddPolicy(
                    appSettings.Value.Cors.PolicyName,
                    policy =>
                    {
                        policy.WithOrigins(appSettings.Value.Cors.AllowedOrigins)
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

                // trace PostgreSQL database queries
                tracing.AddNpgsql();

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
