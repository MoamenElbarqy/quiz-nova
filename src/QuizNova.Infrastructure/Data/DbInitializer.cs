using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

using Npgsql;

using QuizNova.Domain.Common.Results;
using QuizNova.Domain.Entities.Courses;
using QuizNova.Domain.Entities.Quizzes;
using QuizNova.Domain.Entities.Quizzes.Questions.AutoGradedQuestions.Mcq;
using QuizNova.Domain.Entities.Quizzes.Questions.AutoGradedQuestions.Mcq.Choices;
using QuizNova.Domain.Entities.Quizzes.Questions.AutoGradedQuestions.TrueFalse;
using QuizNova.Domain.Entities.Quizzes.Questions.Base;
using QuizNova.Domain.Entities.Quizzes.Questions.ManuallyGradedQuestions;
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

        await EnsureOutboxTriggerAsync(ct);

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

        if (!await dbContext.Quizzes.AnyAsync(ct))
        {
            await SeedQuizzesAsync(ct);
        }

        if (!await dbContext.Enrollments.AnyAsync(ct))
        {
            await SeedEnrollmentsAsync(ct);
        }
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

    private async Task SeedQuizzesAsync(CancellationToken ct)
    {
        var course = await dbContext.Courses.FirstOrDefaultAsync(c => c.Name == "Backend Fundamentals", ct);
        if (course is null)
        {
            return;
        }

        if (!course.InstructorId.HasValue || course.InstructorId.Value == Guid.Empty)
        {
            return;
        }

        var instructorId = course.InstructorId.Value;

        var startsAt = DateTimeOffset.UtcNow.AddMinutes(-30);
        var endsAt = DateTimeOffset.UtcNow.AddHours(3);

        // 1. Auto-graded Only Quiz
        var autoQuizId = Guid.NewGuid();
        var autoQ1Id = Guid.NewGuid();
        var autoQ2Id = Guid.NewGuid();
        var autoQ3Id = Guid.NewGuid();

        var q2Choice1Id = Guid.NewGuid();
        var q2Choice2Id = Guid.NewGuid();
        var q2Choice3Id = Guid.NewGuid();
        var q2Choice4Id = Guid.NewGuid();
        var q2Choices = new List<Choice>
        {
            EnsureSuccess(Choice.Create(q2Choice1Id, autoQ2Id, "public", 0), "q2 choice 1"),
            EnsureSuccess(Choice.Create(q2Choice2Id, autoQ2Id, "private", 1), "q2 choice 2"),
            EnsureSuccess(Choice.Create(q2Choice3Id, autoQ2Id, "internal", 2), "q2 choice 3"),
            EnsureSuccess(Choice.Create(q2Choice4Id, autoQ2Id, "global", 3), "q2 choice 4"),
        };

        var q3Choice1Id = Guid.NewGuid();
        var q3Choice2Id = Guid.NewGuid();
        var q3Choice3Id = Guid.NewGuid();
        var q3Choices = new List<Choice>
        {
            EnsureSuccess(Choice.Create(q3Choice1Id, autoQ3Id, "To compile C# code to IL", 0), "q3 choice 1"),
            EnsureSuccess(Choice.Create(q3Choice2Id, autoQ3Id, "To perform Object-Relational Mapping (ORM)", 1),
                "q3 choice 2"),
            EnsureSuccess(Choice.Create(q3Choice3Id, autoQ3Id, "To design user interfaces", 2), "q3 choice 3"),
        };

        var autoQuestions = new List<Question>
        {
            EnsureSuccess(Tf.Create(autoQ1Id, autoQuizId, "TypeScript is a superset of JavaScript.", true, 0, 2),
                "auto q1"),
            EnsureSuccess(
                Mcq.Create(autoQ2Id, autoQuizId, "Which of the following is not a C# access modifier?", q2Choice4Id, 1,
                    4, q2Choices), "auto q2"),
            EnsureSuccess(
                Mcq.Create(autoQ3Id, autoQuizId, "What is the primary purpose of Entity Framework Core?", q3Choice2Id,
                    2, 4, q3Choices), "auto q3"),
        };

        var autoQuiz =
            EnsureSuccess(
                Quiz.Create(autoQuizId, course.Id, instructorId, "Auto Graded Quiz", startsAt, endsAt,
                    autoQuestions),
                "auto quiz");
        await dbContext.Quizzes.AddAsync(autoQuiz, ct);

        // 2. Manually-graded Only Quiz
        var manualQuizId = Guid.NewGuid();
        var manualQ1Id = Guid.NewGuid();
        var manualQ2Id = Guid.NewGuid();
        var manualQ3Id = Guid.NewGuid();

        var manualQuestions = new List<Question>
        {
            EnsureSuccess(
                Essay.Create(manualQ1Id, manualQuizId,
                    "Explain the difference between interface and abstract class in C#.",
                    "An interface defines a contract with no implementation, while an abstract class can provide partial implementation and state.",
                    0, 5), "manual q1"),
            EnsureSuccess(
                Essay.Create(manualQ2Id, manualQuizId, "Describe the three main stages of the HTTP Request Lifecycle.",
                    "Request initiation/routing, processing/middleware pipeline, and response generation.", 1, 5),
                "manual q2"),
            EnsureSuccess(
                Essay.Create(manualQ3Id, manualQuizId,
                    "What is the Outbox Pattern and how does it guarantee eventual consistency?",
                    "It persists domain events to an Outbox table in the same transaction as state changes, and a background worker publishes them reliably.",
                    2, 5), "manual q3"),
        };

        var manualQuiz =
            EnsureSuccess(
                Quiz.Create(manualQuizId, course.Id, instructorId, "Manual Graded Quiz", startsAt, endsAt,
                    manualQuestions), "manual quiz");
        await dbContext.Quizzes.AddAsync(manualQuiz, ct);

        // 3. Hybrid Graded Quiz
        var hybridQuizId = Guid.NewGuid();
        var hybridQ1Id = Guid.NewGuid();
        var hybridQ2Id = Guid.NewGuid();
        var hybridQ3Id = Guid.NewGuid();

        var hybridQ1Choice1Id = Guid.NewGuid();
        var hybridQ1Choice2Id = Guid.NewGuid();
        var hybridQ1Choice3Id = Guid.NewGuid();
        var hybridQ1Choices = new List<Choice>
        {
            EnsureSuccess(Choice.Create(hybridQ1Choice1Id, hybridQ1Id, "POST", 0), "hybrid q1 choice 1"),
            EnsureSuccess(Choice.Create(hybridQ1Choice2Id, hybridQ1Id, "GET", 1), "hybrid q1 choice 2"),
            EnsureSuccess(Choice.Create(hybridQ1Choice3Id, hybridQ1Id, "PATCH", 2), "hybrid q1 choice 3"),
        };

        var hybridQuestions = new List<Question>
        {
            EnsureSuccess(
                Mcq.Create(hybridQ1Id, hybridQuizId, "Which HTTP method is idempotent?", hybridQ1Choice2Id, 0, 3,
                    hybridQ1Choices), "hybrid q1"),
            EnsureSuccess(
                Tf.Create(hybridQ2Id, hybridQuizId, "In C#, a class can implement multiple interfaces.", true, 1, 2),
                "hybrid q2"),
            EnsureSuccess(
                Essay.Create(hybridQ3Id, hybridQuizId, "Discuss the benefits of Dependency Injection in ASP.NET Core.",
                    "Loose coupling, improved testability, and easier lifetime management of services.", 2, 5),
                "hybrid q3"),
        };

        var hybridQuiz =
            EnsureSuccess(
                Quiz.Create(hybridQuizId, course.Id, instructorId, "Hybrid Graded Quiz", startsAt, endsAt,
                    hybridQuestions), "hybrid quiz");
        await dbContext.Quizzes.AddAsync(hybridQuiz, ct);

        await dbContext.SaveChangesAsync(ct);
    }

    private async Task SeedEnrollmentsAsync(CancellationToken ct)
    {
        var students = await dbContext.Students.ToListAsync(ct);
        var courses = await dbContext.Courses.ToListAsync(ct);

        foreach (var student in students)
        {
            foreach (var course in courses)
            {
                EnsureSuccess(
                    course.Enroll(student),
                    $"enrollment for {student.PersonalInformation.Email} in {course.Name}");
            }
        }

        await dbContext.SaveChangesAsync(ct);
    }

    private async Task EnsureOutboxTriggerAsync(CancellationToken ct)
    {
        await using var connection = new NpgsqlConnection(dbContext.Database.GetConnectionString());
        await connection.OpenAsync(ct);

        await using var createFunction = connection.CreateCommand();
        createFunction.CommandText = """
            CREATE OR REPLACE FUNCTION notify_outbox_insert()
            RETURNS trigger
            LANGUAGE plpgsql
            AS $$
            BEGIN
                PERFORM pg_notify('outbox_channel', NEW."Id"::text);
                RETURN NEW;
            END;
            $$;
            """;
        await createFunction.ExecuteNonQueryAsync(ct);

        await using var dropTrigger = connection.CreateCommand();
        dropTrigger.CommandText = """
            DROP TRIGGER IF EXISTS outbox_insert_trigger ON "OutboxMessages";
            """;
        await dropTrigger.ExecuteNonQueryAsync(ct);

        await using var createTrigger = connection.CreateCommand();
        createTrigger.CommandText = """
            CREATE TRIGGER outbox_insert_trigger
                AFTER INSERT ON "OutboxMessages"
                FOR EACH ROW
                EXECUTE FUNCTION notify_outbox_insert();
            """;
        await createTrigger.ExecuteNonQueryAsync(ct);
    }
}
