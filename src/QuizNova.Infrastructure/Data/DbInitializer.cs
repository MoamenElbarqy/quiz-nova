using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

using QuizNova.Domain.Common.Results;
using QuizNova.Domain.Entities.Courses;
using QuizNova.Domain.Entities.Users.Admins;
using QuizNova.Domain.Entities.Users.Instructors;
using QuizNova.Domain.Entities.Users.Student;
using QuizNova.Domain.Entities.Users.UserPersonalInformation;
using QuizNova.Infrastructure.Identity;

namespace QuizNova.Infrastructure.Data;

public sealed class DbInitializer(
    AppDbContext dbContext,
    UserManager<AppUser> userManager,
    RoleManager<IdentityRole> roleManager)
{
    public async Task InitializeAsync(CancellationToken ct = default)
    {
        await dbContext.Database.MigrateAsync(ct);

        // Seed Roles
        var roles = new[] { "Admin", "Instructor", "Student" };
        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        if (!await dbContext.Admins.AnyAsync(ct))
        {
            var adminId = await SeedIdentityUserAsync("admin@quiznova.local", "Admin123!", "Admin");
            var admin = CreateAdmin(adminId);
            await dbContext.Admins.AddAsync(admin, ct);
            await dbContext.SaveChangesAsync(ct);
        }

        if (!await dbContext.Instructors.AnyAsync(ct))
        {
            var instructors = await CreateInstructorsAsync();
            await dbContext.Instructors.AddRangeAsync(instructors, ct);
            await dbContext.SaveChangesAsync(ct);
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
                await dbContext.SaveChangesAsync(ct);
            }
        }

        if (!await dbContext.Students.AnyAsync(ct))
        {
            await SeedStudentsAsync(ct);
        }

        await dbContext.SaveChangesAsync(ct);
    }

    private async Task<Guid> SeedIdentityUserAsync(string email, string password, string role)
    {
        var appUser = await userManager.FindByEmailAsync(email);
        if (appUser is null)
        {
            appUser = new AppUser
            {
                Id = Guid.NewGuid().ToString(),
                UserName = email,
                Email = email,
                EmailConfirmed = true,
            };
            var result = await userManager.CreateAsync(appUser, password);
            if (!result.Succeeded)
            {
                throw new InvalidOperationException(
                    $"Failed to create seeded user {email}: {result.Errors.First().Description}");
            }

            await userManager.AddToRoleAsync(appUser, role);
        }

        return Guid.Parse(appUser.Id);
    }

    private Admin CreateAdmin(Guid id)
    {
        var personalInfo = EnsureSuccess(
            PersonalInformation.Create(
                name: "Admin User",
                email: "admin@quiznova.local",
                phoneNumber: "01000000000"),
            "admin personal information");

        return EnsureSuccess(
            Admin.Create(id, personalInfo),
            "admin");
    }

    private async Task<List<Instructor>> CreateInstructorsAsync()
    {
        var i1Id = await SeedIdentityUserAsync("instructor1@quiznova.local", "Instructor123!", "Instructor");
        var instructorOneInfo = EnsureSuccess(
            PersonalInformation.Create(
                name: "Instructor One",
                email: "instructor1@quiznova.local",
                phoneNumber: "01000000001"),
            "instructor one personal information");

        var i2Id = await SeedIdentityUserAsync("instructor2@quiznova.local", "Instructor123!", "Instructor");
        var instructorTwoInfo = EnsureSuccess(
            PersonalInformation.Create(
                name: "Instructor Two",
                email: "instructor2@quiznova.local",
                phoneNumber: "01000000002"),
            "instructor two personal information");

        return
        [
            EnsureSuccess(
                Instructor.Create(
                    i1Id,
                    instructorOneInfo,
                    [],
                    []),
                "instructor one"),
            EnsureSuccess(
                Instructor.Create(
                    i2Id,
                    instructorTwoInfo,
                    [],
                    []),
                "instructor two"),
        ];
    }

    private List<Course> CreateCourses(List<Instructor> instructors)
    {
        return
        [
            EnsureSuccess(
                Course.Create(
                    instructors[0].Id,
                    "Backend Fundamentals",
                    minimumPassingMarks: 50,
                    maximumMarks: 100,
                    quizzes: [],
                    enrollments: []),
                "course one"),
            EnsureSuccess(
                Course.Create(
                    instructors[1].Id,
                    "Frontend Fundamentals",
                    minimumPassingMarks: 50,
                    maximumMarks: 100,
                    quizzes: [],
                    enrollments: []),
                "course two"),
        ];
    }

    private async Task SeedStudentsAsync(CancellationToken ct)
    {
        await CreateStudentAsync("Student One", "student1@quiznova.local", "01000000011", ct);
        await CreateStudentAsync("Student Two", "student2@quiznova.local", "01000000012", ct);
        await CreateStudentAsync("Student Three", "student3@quiznova.local", "01000000013", ct);
        await CreateStudentAsync("Student Four", "student4@quiznova.local", "01000000014", ct);
    }

    private async Task CreateStudentAsync(string name, string email, string phoneNumber, CancellationToken ct)
    {
        var studentId = await SeedIdentityUserAsync(email, "Student123!", "Student");
        var personalInfo = EnsureSuccess(
            PersonalInformation.Create(name, email, phoneNumber),
            $"{name} personal information");

        var student = EnsureSuccess(
            Student.Create(
                studentId,
                personalInfo,
                [],
                []),
            name);

        await dbContext.Students.AddAsync(student, ct);
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
