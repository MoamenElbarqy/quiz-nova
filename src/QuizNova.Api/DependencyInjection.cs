using System.Text.Json.Serialization;

using Asp.Versioning;

using Microsoft.AspNetCore.ResponseCompression;

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
        IConfiguration configuration,
        AppSettings appSettings)
    {
        services.AddControllerWithJsonConfiguration();
        services.AddCustomVersioning();
        services.AddApiDocumentation();
        services.AddConfiguredCors(appSettings);
        services.AddExceptionHandling();
        services.AddProblemDetails();
        services.AddAuthorization();
        services.AddAppOpenTelemetry();
        services.AddOutputCache();
        services.AddCustomResponseCompression();
        services.AddIdentityInfrastructure();
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
        services.AddControllers().AddJsonOptions(options => options
            .JsonSerializerOptions
            .DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull);

        return services;
    }

    private static IServiceCollection AddConfiguredCors(this IServiceCollection services, AppSettings appSettings)
    {
        services.AddCors(options =>
        {
            options.AddPolicy(
                appSettings.Cors.PolicyName,
                policy =>
                {
                    policy.WithOrigins(appSettings.Cors.AllowedOrigins)
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

                // export the traces to the endpoint defined in OTEL_EXPORTER_OTLP_ENDPOINT defined in compose that will go to the seq
                tracing.AddOtlpExporter();
            }).WithMetrics(metrics =>
            {
                metrics.AddAspNetCoreInstrumentation().AddHttpClientInstrumentation();

                // export the metrics to /metrics and prometheus will scrape this endpoint
                metrics.AddPrometheusExporter();
            });

        return services;
    }
}
