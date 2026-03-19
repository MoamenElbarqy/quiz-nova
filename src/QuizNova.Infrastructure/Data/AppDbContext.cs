using MediatR;

using Microsoft.EntityFrameworkCore;

using QuizNova.Application.Common.Interfaces;
using QuizNova.Domain.Common;
using QuizNova.Domain.Entities.Colleges;
using QuizNova.Domain.Entities.Courses;
using QuizNova.Domain.Entities.DepartmentCourses;
using QuizNova.Domain.Entities.Departments;
using QuizNova.Domain.Entities.Identity;
using QuizNova.Domain.Entities.Levels;
using QuizNova.Domain.Entities.QuizAttempts;

namespace QuizNova.Infrastructure.Data;

public class AppDbContext(IMediator mediator) : DbContext, IAppDbContext
{
    public DbSet<College> Colleges => Set<College>();

    public DbSet<Course> Courses => Set<Course>();

    public DbSet<DepartmentCourse> DepartmentCourses => Set<DepartmentCourse>();

    public DbSet<Department> Departments => Set<Department>();

    public DbSet<Level> Levels => Set<Level>();

    public DbSet<QuizAttempt> QuizAttempts => Set<QuizAttempt>();

    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

    public override async Task<int> SaveChangesAsync(CancellationToken ct = default)
    {
        await DispatchDomainEventsAsync(ct);
        return await base.SaveChangesAsync(ct);
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

        foreach (var domainEvent in domainEvents)
        {
            await mediator.Publish(domainEvent, ct);
        }

        foreach (var entity in domainEntities)
        {
            entity.ClearDomainEvents();
        }
    }
}
