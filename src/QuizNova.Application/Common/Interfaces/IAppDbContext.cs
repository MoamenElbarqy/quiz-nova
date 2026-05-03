using Microsoft.EntityFrameworkCore;

using QuizNova.Domain.Entities.Courses;
using QuizNova.Domain.Entities.Identity;
using QuizNova.Domain.Entities.QuizAttempts;
using QuizNova.Domain.Entities.Quizzes;
using QuizNova.Domain.Entities.Quizzes.Questions.Base;
using QuizNova.Domain.Entities.Quizzes.Questions.Mcq.Choices;
using QuizNova.Domain.Entities.StudentCourses;
using QuizNova.Domain.Entities.Users;
using QuizNova.Domain.Entities.Users.Admins;
using QuizNova.Domain.Entities.Users.Instructors;
using QuizNova.Domain.Entities.Users.Student;

namespace QuizNova.Application.Common.Interfaces;

public interface IAppDbContext
{
    public DbSet<User> Users { get; }

    public DbSet<Course> Courses { get; }

    public DbSet<QuizAttempt> QuizAttempts { get; }

    public DbSet<Quiz> Quizzes { get; }

    public DbSet<Question> Questions { get; }

    public DbSet<Choice> Choices { get; }

    public DbSet<RefreshToken> RefreshTokens { get; }

    public DbSet<Instructor> Instructors { get; }

    public DbSet<Student> Students { get; }

    public DbSet<Admin> Admins { get; }

    public DbSet<StudentCourse> StudentCourses { get; }

    Task<int> SaveChangesAsync(CancellationToken ct);
}
