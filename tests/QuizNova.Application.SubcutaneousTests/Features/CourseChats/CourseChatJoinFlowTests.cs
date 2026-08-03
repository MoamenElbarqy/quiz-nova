using FluentAssertions;

using Microsoft.Extensions.DependencyInjection;

using MongoDB.Driver;

using QuizNova.Application.Common.Interfaces;
using QuizNova.Application.Features.Courses.Commands.CreateCourse;
using QuizNova.Application.Features.Enrollments.Commands.DisenrollStudentFromCourse;
using QuizNova.Application.Features.Enrollments.Commands.EnrollStudentInCourse;
using QuizNova.Application.SubcutaneousTests.Common;
using QuizNova.Domain.Entities.CourseChats;
using QuizNova.Domain.Entities.Courses;
using QuizNova.Domain.Entities.Courses.Events;
using QuizNova.Domain.Entities.Enrollments.Events;
using QuizNova.Domain.Entities.Identity;
using QuizNova.Domain.Entities.Users.Admins;
using QuizNova.Domain.Entities.Users.UserPersonalInformation;
using QuizNova.Tests.Common.Security;
using QuizNova.Tests.Common.Users.Students;
using QuizNova.Tests.Common.Users.UserPersonalInformation;

namespace QuizNova.Application.SubcutaneousTests.Features.CourseChats;

public class CourseChatJoinFlowTests(CustomWebApplicationFactory factory)
    : IClassFixture<CustomWebApplicationFactory>
{
    [Fact]
    public async Task CreateCourse_ShouldAutomaticallyCreateCourseChatRoom()
    {
        // Arrange
        var mediator = factory.CreateMediator();

        EnsureAdminContext();

        Guid instructorId;
        using (var scope = factory.Services.CreateScope())
        {
            var mongoContext = scope.ServiceProvider.GetRequiredService<IMongoDbContext>();
            var instructor = await mongoContext.Users.Find(u => u.UserRole == UserRole.Instructor).FirstAsync();
            instructorId = instructor.Id;
        }

        var command = new CreateCourseCommand(
            Name: $"Course {Guid.NewGuid().ToString()[..8]}",
            InstructorId: instructorId,
            MinimumPassingMarks: 50,
            MaximumMarks: 100);

        // Act
        var result = await mediator.Send(command);

        // Assert
        result.IsSuccess.Should().BeTrue();
        var courseId = result.Value.Id;

        await mediator.Publish(new CourseCreatedEvent(courseId));

        using (var scope = factory.Services.CreateScope())
        {
            var mongoContext = scope.ServiceProvider.GetRequiredService<IMongoDbContext>();
            var chatRoom = await mongoContext.CourseChatRooms.Find(r => r.CourseId == courseId).FirstOrDefaultAsync();

            chatRoom.Should().NotBeNull();
            chatRoom.CourseId.Should().Be(courseId);
            chatRoom.InstructorId.Should().Be(instructorId);
        }
    }

    [Fact]
    public async Task EnrollStudent_ShouldAddStudentToCourseChatRoom()
    {
        // Arrange
        var mediator = factory.CreateMediator();

        Guid courseId;
        Guid studentId;
        using (var scope = factory.Services.CreateScope())
        {
            var mongoContext = scope.ServiceProvider.GetRequiredService<IMongoDbContext>();

            var instructor = await mongoContext.Users.Find(u => u.UserRole == UserRole.Instructor).FirstAsync();
            var course = Course.Create(instructor.Id, $"Course {Guid.NewGuid().ToString()[..8]}", 50, 100).Value;
            await mongoContext.Courses.InsertOneAsync(course);
            courseId = course.Id;

            var personalInfo = PersonalInformationFactory.CreatePersonalInformation(
                name: $"Student {Guid.NewGuid().ToString()[..8]}",
                email: $"student.{Guid.NewGuid().ToString()[..8]}@example.com");
            var student = StudentFactory.CreateStudent(personalInformation: personalInfo).Value;
            await mongoContext.Users.InsertOneAsync(student);
            studentId = student.Id;

            var room = CourseChatRoom.Create(courseId, instructor.Id).Value;
            await mongoContext.CourseChatRooms.InsertOneAsync(room);
        }

        // Act
        EnsureAdminContext();
        var enrollCommand = new EnrollStudentInCourseCommand(courseId, studentId);
        var enrollResult = await mediator.Send(enrollCommand);
        enrollResult.IsSuccess.Should().BeTrue();

        await mediator.Publish(new StudentEnrolledEvent(courseId, studentId));

        // Assert
        using (var scope = factory.Services.CreateScope())
        {
            var mongoContext = scope.ServiceProvider.GetRequiredService<IMongoDbContext>();
            var chatRoom = await mongoContext.CourseChatRooms.Find(r => r.CourseId == courseId).FirstOrDefaultAsync();

            chatRoom.Should().NotBeNull();
            chatRoom.StudentIds.Should().Contain(studentId);
        }
    }

    [Fact]
    public async Task RemoveStudent_ShouldRemoveStudentFromCourseChatRoom()
    {
        // Arrange
        var mediator = factory.CreateMediator();

        Guid courseId;
        Guid studentId;
        Guid enrollmentId;
        using (var scope = factory.Services.CreateScope())
        {
            var mongoContext = scope.ServiceProvider.GetRequiredService<IMongoDbContext>();

            var instructor = await mongoContext.Users.Find(u => u.UserRole == UserRole.Instructor).FirstAsync();
            var course = Course.Create(instructor.Id, $"Course {Guid.NewGuid().ToString()[..8]}", 50, 100).Value;
            await mongoContext.Courses.InsertOneAsync(course);
            courseId = course.Id;

            var personalInfo = PersonalInformationFactory.CreatePersonalInformation(
                name: $"Student {Guid.NewGuid().ToString()[..8]}",
                email: $"student.{Guid.NewGuid().ToString()[..8]}@example.com");
            var student = StudentFactory.CreateStudent(personalInformation: personalInfo).Value;
            await mongoContext.Users.InsertOneAsync(student);
            studentId = student.Id;

            var room = CourseChatRoom.Create(courseId, instructor.Id).Value;
            await mongoContext.CourseChatRooms.InsertOneAsync(room);

            EnsureAdminContext();
            var enrollResult = await mediator.Send(new EnrollStudentInCourseCommand(courseId, studentId));
            enrollResult.IsSuccess.Should().BeTrue();
            await mediator.Publish(new StudentEnrolledEvent(courseId, studentId));

            var enrollment = await mongoContext.Enrollments.Find(e => e.CourseId == courseId && e.StudentId == studentId).FirstAsync();
            enrollmentId = enrollment.Id;
        }

        using (var scope = factory.Services.CreateScope())
        {
            var mongoContext = scope.ServiceProvider.GetRequiredService<IMongoDbContext>();
            var chatRoom = await mongoContext.CourseChatRooms.Find(r => r.CourseId == courseId).FirstOrDefaultAsync();
            chatRoom!.StudentIds.Should().Contain(studentId);
        }

        // Act
        var removeCommand = new DisenrollStudentFromCourseCommand(enrollmentId, studentId);
        var removeResult = await mediator.Send(removeCommand);
        removeResult.IsSuccess.Should().BeTrue();

        await mediator.Publish(new StudentDisenrolledEvent(studentId, courseId));

        // Assert
        using (var scope = factory.Services.CreateScope())
        {
            var mongoContext = scope.ServiceProvider.GetRequiredService<IMongoDbContext>();
            var chatRoom = await mongoContext.CourseChatRooms.Find(r => r.CourseId == courseId).FirstOrDefaultAsync();

            chatRoom.Should().NotBeNull();
            chatRoom.StudentIds.Should().NotContain(studentId);
        }
    }

    private void EnsureAdminContext()
    {
        var adminId = Guid.Parse(TestUsers.Admin.User.Id);
        using var scope = factory.Services.CreateScope();
        var mongoContext = scope.ServiceProvider.GetRequiredService<IMongoDbContext>();
        if (!mongoContext.Users.Find(u => u.UserRole == UserRole.Admin && u.Id == adminId).Any())
        {
            var personalInfo = PersonalInformation.Create("Admin User", "admin@quiznova.local", "01000000000").Value;
            var admin = Admin.Create(adminId, personalInfo).Value;
            mongoContext.Users.InsertOne(admin);
        }

        TestCurrentUser.Set(TestUsers.Admin.User);
    }
}
