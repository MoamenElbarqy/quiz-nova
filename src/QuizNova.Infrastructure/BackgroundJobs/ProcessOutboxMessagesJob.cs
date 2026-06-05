using System.Text.Json;

using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using QuizNova.Domain.Common;
using QuizNova.Infrastructure.Data;

namespace QuizNova.Infrastructure.BackgroundJobs;

public class ProcessOutboxMessagesJob(
    IServiceScopeFactory scopeFactory,
    ILogger<ProcessOutboxMessagesJob> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        logger.LogInformation("Outbox background processor started.");

        while (!ct.IsCancellationRequested)
        {
            try
            {
                await ProcessNextMessageAsync(ct);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error occurred while processing outbox messages.");
            }

            await Task.Delay(TimeSpan.FromSeconds(0.5), ct);
        }
    }

    private async Task ProcessNextMessageAsync(CancellationToken ct)
    {
        using var scope = scopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        // Open a transaction block to hold the row lock
        await using var transaction = await dbContext.Database.BeginTransactionAsync(ct);

        var message = await dbContext.OutboxMessages
            .FromSqlRaw("""
                        SELECT * FROM "OutboxMessages"
                        WHERE "ProcessedOnUtc" IS NULL
                        ORDER BY "OccurredOnUtc" ASC
                        LIMIT 1
                        FOR UPDATE SKIP LOCKED
                        """)
            .SingleOrDefaultAsync(ct);

        if (message == null)
        {
            await transaction.RollbackAsync(ct);
            return;
        }

        try
        {
            // 1. Reconstruct the original Domain Event type
            var assembly = typeof(DomainEvent).Assembly;
            var type = assembly.GetTypes().FirstOrDefault(t => t.Name == message.Type);

            if (type == null)
            {
                throw new Exception($"Unknown event type: {message.Type}");
            }

            var domainEvent = JsonSerializer.Deserialize(message.Content, type);
            if (domainEvent is INotification notification)
            {
                // 2. Publish it to MediatR handlers (e.g., CourseDeletedCacheInvalidationHandler)
                await mediator.Publish(notification, ct);
            }

            // 3. Mark message as successfully processed
            message.ProcessedOnUtc = DateTime.UtcNow;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to process outbox message {MessageId}", message.Id);
            message.Error = ex.ToString();
        }

        // Save status change and commit the transaction to release the lock
        await dbContext.SaveChangesAsync(ct);
        await transaction.CommitAsync(ct);
    }
}
