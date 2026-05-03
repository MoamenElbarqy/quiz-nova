using Microsoft.EntityFrameworkCore;

using QuizNova.Domain.Common.Results;
using QuizNova.Domain.Entities.Courses;
using QuizNova.Domain.Entities.Identity;
using QuizNova.Domain.Entities.QuizAttempts;
using QuizNova.Domain.Entities.Quizzes;
using QuizNova.Domain.Entities.StudentCourses;
using QuizNova.Domain.Entities.Users.Admins;
using QuizNova.Domain.Entities.Users.Instructors;
using QuizNova.Domain.Entities.Users.Student;
using QuizNova.Domain.Entities.Users.UserPersonalInformation;

namespace QuizNova.Infrastructure.Data;

public sealed class DbInitializer(AppDbContext dbContext)
{
    public async Task InitializeAsync(CancellationToken ct = default)
    {
        await dbContext.Database.EnsureCreatedAsync(ct);

        if (!await dbContext.Admins.AnyAsync(ct))
        {
            var admin = CreateAdmin();
            await dbContext.Admins.AddAsync(admin, ct);
        }

        if (!await dbContext.Instructors.AnyAsync(ct))
        {
            var instructors = CreateInstructors();
            await dbContext.Instructors.AddRangeAsync(instructors, ct);
        }

        if (!await dbContext.Courses.AnyAsync(ct))
        {
            var instructors = await dbContext.Instructors
                .OrderBy(instructor => instructor.PersonalInformation.Email)
                .ToListAsync(ct);

            if (instructors.Count >= 2)
            {
                var courses = CreateCourses(instructors);
                await dbContext.Courses.AddRangeAsync(courses, ct);
            }
        }

        if (!await dbContext.Students.AnyAsync(ct))
        {
            var students = CreateStudents();
            await dbContext.Students.AddRangeAsync(students, ct);
        }

        await dbContext.SaveChangesAsync(ct);
    }

    private Admin CreateAdmin()
    {
        var personalInfo = EnsureSuccess(
            PersonalInformation.Create(
                name: "Admin User",
                email: "admin@quiznova.local",
                password: "Admin123!",
                phoneNumber: "01000000000"),
            "admin personal information");

        return EnsureSuccess(
            Admin.Create(Guid.NewGuid(), personalInfo, new List<RefreshToken>()),
            "admin");
    }

    private List<Instructor> CreateInstructors()
    {
        var instructorOneInfo = EnsureSuccess(
            PersonalInformation.Create(
                name: "Instructor One",
                email: "instructor1@quiznova.local",
                password: "Instructor123!",
                phoneNumber: "01000000001"),
            "instructor one personal information");

        var instructorTwoInfo = EnsureSuccess(
            PersonalInformation.Create(
                name: "Instructor Two",
                email: "instructor2@quiznova.local",
                password: "Instructor123!",
                phoneNumber: "01000000002"),
            "instructor two personal information");

        return
        [
            EnsureSuccess(
                Instructor.Create(
                    Guid.NewGuid(),
                    instructorOneInfo,
                    new List<RefreshToken>(),
                    new List<Course>(),
                    new List<Quiz>()),
                "instructor one"),
            EnsureSuccess(
                Instructor.Create(
                    Guid.NewGuid(),
                    instructorTwoInfo,
                    new List<RefreshToken>(),
                    new List<Course>(),
                    new List<Quiz>()),
                "instructor two"),
        ];
    }

    private List<Course> CreateCourses(List<Instructor> instructors)
    {
        return
        [
            EnsureSuccess(
                Course.Create(
                    Guid.NewGuid(),
                    instructors[0].Id,
                    "Backend Fundamentals",
                    minimumPassingMarks: 50,
                    maximumMarks: 100,
                    quizzes: new List<Quiz>()),
                "course one"),
            EnsureSuccess(
                Course.Create(
                    Guid.NewGuid(),
                    instructors[1].Id,
                    "Frontend Fundamentals",
                    minimumPassingMarks: 50,
                    maximumMarks: 100,
                    quizzes: new List<Quiz>()),
                "course two"),
        ];
    }

    private List<Student> CreateStudents()
    {
        return
        [
            CreateStudent("Student One", "student1@quiznova.local", "01000000011"),
            CreateStudent("Student Two", "student2@quiznova.local", "01000000012"),
            CreateStudent("Student Three", "student3@quiznova.local", "01000000013"),
            CreateStudent("Student Four", "student4@quiznova.local", "01000000014"),
        ];
    }

    private Student CreateStudent(string name, string email, string phoneNumber)
    {
        var personalInfo = EnsureSuccess(
            PersonalInformation.Create(name, email, "Student123!", phoneNumber),
            $"{name} personal information");

        return EnsureSuccess(
            Student.Create(
                Guid.NewGuid(),
                personalInfo,
                new List<RefreshToken>(),
                new List<StudentCourse>(),
                new List<QuizAttempt>()),
            name);
    }

    private T EnsureSuccess<T>(Result<T> result, string entityName)
    {
        if (result.IsError)
        {
            var error = result.TopError;
            throw new InvalidOperationException($"Failed to create {entityName}: {error.Code} - {error.Description}");
        }

        return result.Value;
    }
}
