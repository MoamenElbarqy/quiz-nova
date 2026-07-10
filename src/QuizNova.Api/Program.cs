using Microsoft.Extensions.Options;

using QuizNova.Api;
using QuizNova.Api.Hubs;
using QuizNova.Infrastructure.Data;
using QuizNova.Infrastructure.Settings;

using Scalar.AspNetCore;

using Serilog;

Serilog.Debugging.SelfLog.Enable(Console.WriteLine);

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, loggerConfig) =>
    loggerConfig.ReadFrom.Configuration(context.Configuration));

builder.Services.AddApplication();

builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddApi(builder.Configuration);

var app = builder.Build();

app.UseForwardedHeaders();

app.UseWebSockets();

app.UseResponseCompression();

app.UseExceptionHandler();

app.UseOutputCache();

var appSettings = app.Services.GetRequiredService<IOptions<AppSettings>>().Value;
app.UseCors(appSettings.Cors.PolicyName);

app.UseAuthentication();
app.UseAuthorization();

app.UseRateLimiter();

if (app.Configuration.GetValue<bool>("ResetDatabase"))
{
    using var scope = app.Services.CreateScope();
    var resetCtx = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await resetCtx.Database.EnsureDeletedAsync();
}

if (app.Environment.IsDevelopment() || app.Configuration.GetValue<bool>("AutoMigrateDb"))
{
    await app.Services.InitializeDevelopmentDatabaseAsync();
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "QuizNova API V1");

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

app.MapHub<ChatHub>("/chat");

// app.MapAllEndpoints();
app.Run();
