using Microsoft.EntityFrameworkCore;

using QuizNova.Domain.Entities.Colleges;
using QuizNova.Domain.Entities.Courses;
using QuizNova.Domain.Entities.DepartmentCourses;
using QuizNova.Domain.Entities.Departments;
using QuizNova.Domain.Entities.Identity;
using QuizNova.Domain.Entities.Levels;
using QuizNova.Domain.Entities.QuizAttempts;

namespace QuizNova.Application.Common.Interfaces;

public interface IAppDbContext
{
    public DbSet<College> Colleges { get; }

    public DbSet<Course> Courses { get; }

    public DbSet<DepartmentCourse> DepartmentCourses { get; }

    public DbSet<Department> Departments { get; }

    public DbSet<Level> Levels { get; }

    public DbSet<QuizAttempt> QuizAttempts { get; }

    public DbSet<RefreshToken> RefreshTokens { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
