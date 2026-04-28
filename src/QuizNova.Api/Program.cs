using QuizNova.Api;
using QuizNova.Infrastructure.Settings;

using Scalar.AspNetCore;

using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, loggerConfig) =>
    loggerConfig.ReadFrom.Configuration(context.Configuration));

var appSettings = builder.Configuration
                      .GetSection(AppSettings.SectionName)
                      .Get<AppSettings>()
                  ?? throw new InvalidOperationException("AppSettings not configured");

builder.Services.AddApplication();

builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddApi(builder.Configuration, appSettings);

var app = builder.Build();

app.UseExceptionHandler();

app.UseCors(appSettings.Cors.PolicyName);

app.UseAuthentication();
app.UseAuthorization();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    await app.Services.InitializeDevelopmentDatabaseAsync();

    app.MapOpenApi();

    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "MechanicShop API V1");

        options.EnableDeepLinking();
        options.DisplayRequestDuration();
        options.EnableFilter();
    });

    app.MapScalarApiReference();
}
else
{
    app.UseHsts();
}

app.MapPrometheusScrapingEndpoint();

app.MapControllers();

app.Run();
