using FluentAssertions;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using QuizNova.Application.Features.Courses.Commands.CreateCourse;
using QuizNova.Application.Features.Enrollments.Commands.EnrollStudentInCourse;
using QuizNova.Application.Features.Enrollments.Commands.RemoveStudentFromCourse;
using QuizNova.Application.SubcutaneousTests.Common;
using QuizNova.Domain.Entities.CourseChats;
using QuizNova.Domain.Entities.Courses;
using QuizNova.Domain.Entities.Courses.Events;
using QuizNova.Domain.Entities.Enrollments.Events;
using QuizNova.Infrastructure.Data;
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

        Guid instructorId;
        using (var scope = factory.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var instructor = await dbContext.Instructors.FirstAsync();
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
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var chatRoom = await dbContext.CourseChatRooms.FirstOrDefaultAsync(r => r.CourseId == courseId);

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
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var instructor = await dbContext.Instructors.FirstAsync();
            var course = Course.Create(instructor.Id, $"Course {Guid.NewGuid().ToString()[..8]}", 50, 100, [], []).Value;
            await dbContext.Courses.AddAsync(course);
            courseId = course.Id;

            var personalInfo = PersonalInformationFactory.CreatePersonalInformation(
                name: $"Student {Guid.NewGuid().ToString()[..8]}",
                email: $"student.{Guid.NewGuid().ToString()[..8]}@example.com");
            var student = StudentFactory.CreateStudent(personalInformation: personalInfo).Value;
            await dbContext.Students.AddAsync(student);
            studentId = student.Id;

            var room = CourseChatRoom.Create(courseId, instructor.Id).Value;
            await dbContext.CourseChatRooms.AddAsync(room);
            await dbContext.SaveChangesAsync();
        }

        // Act
        var enrollCommand = new EnrollStudentInCourseCommand(courseId, studentId);
        var enrollResult = await mediator.Send(enrollCommand);
        enrollResult.IsSuccess.Should().BeTrue();

        await mediator.Publish(new StudentEnrolledEvent(courseId, studentId));

        // Assert
        using (var scope = factory.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var chatRoom = await dbContext.CourseChatRooms
                .Include(r => r.Students)
                .FirstOrDefaultAsync(r => r.CourseId == courseId);

            chatRoom.Should().NotBeNull();
            chatRoom.Students.Should().Contain(s => s.Id == studentId);
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
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var instructor = await dbContext.Instructors.FirstAsync();
            var course = Course.Create(instructor.Id, $"Course {Guid.NewGuid().ToString()[..8]}", 50, 100, [], []).Value;
            await dbContext.Courses.AddAsync(course);
            courseId = course.Id;

            var personalInfo = PersonalInformationFactory.CreatePersonalInformation(
                name: $"Student {Guid.NewGuid().ToString()[..8]}",
                email: $"student.{Guid.NewGuid().ToString()[..8]}@example.com");
            var student = StudentFactory.CreateStudent(personalInformation: personalInfo).Value;
            await dbContext.Students.AddAsync(student);
            studentId = student.Id;

            var room = CourseChatRoom.Create(courseId, instructor.Id).Value;
            await dbContext.CourseChatRooms.AddAsync(room);
            await dbContext.SaveChangesAsync();

            var enrollResult = await mediator.Send(new EnrollStudentInCourseCommand(courseId, studentId));
            enrollResult.IsSuccess.Should().BeTrue();
            await mediator.Publish(new StudentEnrolledEvent(courseId, studentId));

            var enrollment = await dbContext.Enrollments
                .FirstAsync(e => e.CourseId == courseId && e.StudentId == studentId);
            enrollmentId = enrollment.Id;
        }

        using (var scope = factory.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var chatRoom = await dbContext.CourseChatRooms
                .Include(r => r.Students)
                .FirstOrDefaultAsync(r => r.CourseId == courseId);
            chatRoom!.Students.Should().Contain(s => s.Id == studentId);
        }

        // Act
        var removeCommand = new RemoveStudentFromCourseCommand(enrollmentId, studentId);
        var removeResult = await mediator.Send(removeCommand);
        removeResult.IsSuccess.Should().BeTrue();

        await mediator.Publish(new EnrollmentDeletedEvent(studentId, courseId));

        // Assert
        using (var scope = factory.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var chatRoom = await dbContext.CourseChatRooms
                .Include(r => r.Students)
                .FirstOrDefaultAsync(r => r.CourseId == courseId);

            chatRoom.Should().NotBeNull();
            chatRoom.Students.Should().NotContain(s => s.Id == studentId);
        }
    }
}
