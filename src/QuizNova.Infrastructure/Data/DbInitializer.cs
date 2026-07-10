using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

using Npgsql;

using QuizNova.Domain.Common.Results;
using QuizNova.Domain.Entities.CourseChats;
using QuizNova.Domain.Entities.Courses;
using QuizNova.Domain.Entities.Enrollments;
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

        if (!await dbContext.CourseChatRooms.AnyAsync(ct))
        {
            await SeedCourseChatRoomsAsync(ct);
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
        var i1Id = await SeedIdentityUserAsync("ahmed.nasser@quiznova.local", "Instructor123!", "Instructor");
        var instructorOneInfo = EnsureSuccess(
            PersonalInformation.Create(
                name: "Dr. Ahmed Nasser",
                email: "ahmed.nasser@quiznova.local",
                phoneNumber: "01000000001"),
            "instructor one personal information");

        var i2Id = await SeedIdentityUserAsync("sara.kamel@quiznova.local", "Instructor123!", "Instructor");
        var instructorTwoInfo = EnsureSuccess(
            PersonalInformation.Create(
                name: "Dr. Sara Kamel",
                email: "sara.kamel@quiznova.local",
                phoneNumber: "01000000002"),
            "instructor two personal information");

        var i3Id = await SeedIdentityUserAsync("marwan.hosny@quiznova.local", "Instructor123!", "Instructor");
        var instructorThreeInfo = EnsureSuccess(
            PersonalInformation.Create(
                name: "Eng. Marwan Hosny",
                email: "marwan.hosny@quiznova.local",
                phoneNumber: "01000000003"),
            "instructor three personal information");

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
            EnsureSuccess(
                Instructor.Create(
                    i3Id,
                    instructorThreeInfo,
                    [],
                    []),
                "instructor three"),
        ];
    }

    private List<Course> CreateCourses(List<Instructor> instructors)
    {
        return
        [
            EnsureSuccess(
                Course.Create(
                    instructors[0].Id,
                    "Data Structures & Algorithms",
                    minimumPassingMarks: 50,
                    maximumMarks: 500,
                    quizzes: [],
                    enrollments: []),
                "course one"),
            EnsureSuccess(
                Course.Create(
                    instructors[0].Id,
                    "Database Systems",
                    minimumPassingMarks: 50,
                    maximumMarks: 500,
                    quizzes: [],
                    enrollments: []),
                "course two"),
            EnsureSuccess(
                Course.Create(
                    instructors[1].Id,
                    "Web Development Fundamentals",
                    minimumPassingMarks: 50,
                    maximumMarks: 500,
                    quizzes: [],
                    enrollments: []),
                "course three"),
            EnsureSuccess(
                Course.Create(
                    instructors[1].Id,
                    "Software Engineering",
                    minimumPassingMarks: 50,
                    maximumMarks: 500,
                    quizzes: [],
                    enrollments: []),
                "course four"),
            EnsureSuccess(
                Course.Create(
                    instructors[2].Id,
                    "Machine Learning",
                    minimumPassingMarks: 50,
                    maximumMarks: 500,
                    quizzes: [],
                    enrollments: []),
                "course five"),
            EnsureSuccess(
                Course.Create(
                    instructors[2].Id,
                    "Computer Networks",
                    minimumPassingMarks: 50,
                    maximumMarks: 500,
                    quizzes: [],
                    enrollments: []),
                "course six"),
        ];
    }

    private async Task SeedStudentsAsync(CancellationToken ct)
    {
        await CreateStudentAsync("Omar Yasser", "omar.yasser@quiznova.local", "01000000011", ct);
        await CreateStudentAsync("Layla Hassan", "layla.hassan@quiznova.local", "01000000012", ct);
        await CreateStudentAsync("Karim Adel", "karim.adel@quiznova.local", "01000000013", ct);
        await CreateStudentAsync("Nourhan Samir", "nourhan.samir@quiznova.local", "01000000014", ct);
        await CreateStudentAsync("Youssef Ibrahim", "youssef.ibrahim@quiznova.local", "01000000015", ct);
        await CreateStudentAsync("Hana Mahmoud", "hana.mahmoud@quiznova.local", "01000000016", ct);
        await CreateStudentAsync("Ali Mostafa", "ali.mostafa@quiznova.local", "01000000017", ct);
        await CreateStudentAsync("Farida Tamer", "farida.tamer@quiznova.local", "01000000018", ct);
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

    private async Task SeedEnrollmentsAsync(CancellationToken ct)
    {
        var students = await dbContext.Students.ToListAsync(ct);
        var courses = await dbContext.Courses.ToListAsync(ct);

        foreach (var student in students)
        {
            foreach (var course in courses)
            {
                var enrollment = EnsureSuccess(
                    Enrollment.Create(Guid.NewGuid(), student.Id, course.Id, DateTimeOffset.UtcNow),
                    $"enrollment for {student.PersonalInformation.Email} in {course.Name}");
                dbContext.Enrollments.Add(enrollment);
            }
        }

        await dbContext.SaveChangesAsync(ct);
    }

    private async Task SeedCourseChatRoomsAsync(CancellationToken ct)
    {
        var courses = await dbContext.Courses.ToListAsync(ct);

        foreach (var course in courses)
        {
            var chatRoomResult = CourseChatRoom.Create(course.Id, course.InstructorId);
            if (chatRoomResult.IsSuccess)
            {
                await dbContext.CourseChatRooms.AddAsync(chatRoomResult.Value, ct);
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
