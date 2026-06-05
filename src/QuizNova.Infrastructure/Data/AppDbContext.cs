using System.Text.Json;

using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

using QuizNova.Application.Common.Interfaces;
using QuizNova.Domain.Common;
using QuizNova.Domain.Entities.Courses;
using QuizNova.Domain.Entities.Enrollments;
using QuizNova.Domain.Entities.QuizAttempts;
using QuizNova.Domain.Entities.QuizAttempts.Answers.Base;
using QuizNova.Domain.Entities.QuizAttempts.Answers.ManuallyGradedAnswers;
using QuizNova.Domain.Entities.QuizAttempts.Answers.ManuallyGradedAnswers.EssayAnswer;
using QuizNova.Domain.Entities.Quizzes;
using QuizNova.Domain.Entities.Quizzes.Questions.AutoGradedQuestions.Mcq;
using QuizNova.Domain.Entities.Quizzes.Questions.AutoGradedQuestions.Mcq.Choices;
using QuizNova.Domain.Entities.Quizzes.Questions.Base;
using QuizNova.Domain.Entities.Users;
using QuizNova.Domain.Entities.Users.Admins;
using QuizNova.Domain.Entities.Users.Instructors;
using QuizNova.Domain.Entities.Users.Student;
using QuizNova.Infrastructure.Identity;

namespace QuizNova.Infrastructure.Data;

public class AppDbContext(
    DbContextOptions<AppDbContext> options)
    : IdentityDbContext<AppUser>(options), IAppDbContext
{
    public DbSet<Course> Courses => Set<Course>();

    public DbSet<QuizAttempt> QuizAttempts => Set<QuizAttempt>();

    public DbSet<Quiz> Quizzes => Set<Quiz>();

    public DbSet<Question> Questions => Set<Question>();

    public DbSet<QuestionAnswer> QuestionAnswers => Set<QuestionAnswer>();

    public DbSet<ManuallyGradedAnswers> ManuallyGradedAnswers => Set<ManuallyGradedAnswers>();

    public DbSet<EssayAnswer> EssayAnswers => Set<EssayAnswer>();

    public DbSet<Choice> Choices => Set<Choice>();

    public DbSet<UserRefreshToken> UserRefreshTokens => Set<UserRefreshToken>();

    public new DbSet<User> Users => Set<User>();

    public DbSet<Instructor> Instructors => Set<Instructor>();

    public DbSet<Student> Students => Set<Student>();

    public DbSet<Admin> Admins => Set<Admin>();

    public DbSet<Enrollment> Enrollments => Set<Enrollment>();

    public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();

    public override async Task<int> SaveChangesAsync(CancellationToken ct = default)
    {
        await DispatchDomainEventsAsync(ct);

        var addedMcqEntries = ChangeTracker.Entries<Mcq>()
            .Where(e => e.State == EntityState.Added)
            .ToList();

        if (addedMcqEntries.Count == 0)
        {
            return await base.SaveChangesAsync(ct);
        }

        var mcqInfos = addedMcqEntries.Select(entry => new
        {
            Entry = entry,
            entry.Entity.CorrectChoiceId,
            CorrectChoice = entry.Reference(q => q.CorrectChoice).CurrentValue,
        }).ToList();

        foreach (var info in mcqInfos)
        {
            info.Entry.Property(q => q.CorrectChoiceId).CurrentValue = null;
            info.Entry.Reference(q => q.CorrectChoice).CurrentValue = null;
        }

        var result = await base.SaveChangesAsync(ct);

        foreach (var info in mcqInfos)
        {
            info.Entry.Property(q => q.CorrectChoiceId).CurrentValue = info.CorrectChoiceId;
            info.Entry.Reference(q => q.CorrectChoice).CurrentValue = info.CorrectChoice;
            info.Entry.State = EntityState.Modified;
        }

        await base.SaveChangesAsync(ct);

        return result;
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }

    private async Task DispatchDomainEventsAsync(CancellationToken ct)
    {
        var domainEntities = ChangeTracker.Entries()
            .Where(e => e.Entity is Entity baseEntity && baseEntity.DomainEvents.Count != 0)
            .Select(e => (Entity)e.Entity)
            .ToList();

        var domainEvents = domainEntities
            .SelectMany(e => e.DomainEvents)
            .ToList();

        // Convert domain events into Outbox Messages
        var outboxMessages = domainEvents.Select(domainEvent => new OutboxMessage
        {
            Id = Guid.NewGuid(),
            OccurredOnUtc = DateTime.UtcNow,
            Type = domainEvent.GetType().Name,
            Content = JsonSerializer.Serialize(domainEvent, domainEvent.GetType()),
        }).ToList();

        // Save outbox messages to the database
        await OutboxMessages.AddRangeAsync(outboxMessages, ct);

        // Clear events from domain entities
        foreach (var entity in domainEntities)
        {
            entity.ClearDomainEvents();
        }
    }
}
