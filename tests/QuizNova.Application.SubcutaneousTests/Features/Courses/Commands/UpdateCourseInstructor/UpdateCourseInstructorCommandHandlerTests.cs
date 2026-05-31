using FluentAssertions;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using QuizNova.Application.Common.Interfaces;
using QuizNova.Application.Features.Courses.Commands.CreateCourse;
using QuizNova.Application.Features.Courses.Commands.UpdateCourseInstructor;
using QuizNova.Application.Features.Instructors.Commands.CreateInstructor;
using QuizNova.Application.SubcutaneousTests.Common;
using QuizNova.Domain.Entities.Identity;

namespace QuizNova.Application.SubcutaneousTests.Features.Courses.Commands.UpdateCourseInstructor;

public class UpdateCourseInstructorCommandHandlerTests(CustomWebApplicationFactory factory)
    : IClassFixture<CustomWebApplicationFactory>
{
    [Fact]
    public async Task Handle_WithEmptyCourseId_ShouldReturnValidationError()
    {
        // Arrange
        var mediator = factory.CreateMediator();
        var command = new UpdateCourseInstructorCommand(Guid.Empty, null);

        // Act
        var result = await mediator.Send(command);

        // Assert
        result.IsError.Should().BeTrue();
        result.Errors.Should().Contain(e => e.Code == "CourseId");
    }

    [Fact]
    public async Task Handle_WithNonExistentCourseId_ShouldReturnNotFoundError()
    {
        // Arrange
        var mediator = factory.CreateMediator();
        var command = new UpdateCourseInstructorCommand(Guid.NewGuid(), null);

        // Act
        var result = await mediator.Send(command);

        // Assert
        result.IsError.Should().BeTrue();
        result.TopError.Code.Should().Be("Course.NotFound");
    }

    [Fact]
    public async Task Handle_WithNonExistentInstructor_ShouldReturnNotFoundError()
    {
        // Arrange
        var mediator = factory.CreateMediator();

        // 1. Create a course
        var createCourseResult = await mediator.Send(new CreateCourseCommand(
            Name: "Course No Instructor",
            InstructorId: null,
            MinimumPassingMarks: 50,
            MaximumMarks: 100));
        createCourseResult.IsSuccess.Should().BeTrue();
        var courseId = createCourseResult.Value.CourseId;

        // 2. Try to assign a non-existent instructor
        var command = new UpdateCourseInstructorCommand(courseId, Guid.NewGuid());

        // Act
        var result = await mediator.Send(command);

        // Assert
        result.IsError.Should().BeTrue();
        result.TopError.Code.Should().Be("Instructor_NotFound");
    }

    [Fact]
    public async Task Handle_WithValidCourseAndInstructor_ShouldUpdateInstructorSuccessfully()
    {
        // Arrange
        var mediator = factory.CreateMediator();

        // 1. Create an instructor
        var instructorEmail = $"instructor_{Guid.NewGuid()}@example.com";
        var instructorPhone = $"+1{Guid.NewGuid().ToString()[..10]}";
        var instructorResult = await mediator.Send(new CreateInstructorCommand(
            Name: "New Instructor",
            Email: instructorEmail,
            Password: "SecurePass123!",
            PhoneNumber: instructorPhone,
            Role: nameof(UserRole.Instructor)));
        instructorResult.IsSuccess.Should().BeTrue();
        var instructorId = instructorResult.Value.Id;

        // 2. Create a course without instructor
        var courseResult = await mediator.Send(new CreateCourseCommand(
            Name: "Course For Update",
            InstructorId: null,
            MinimumPassingMarks: 60,
            MaximumMarks: 100));
        courseResult.IsSuccess.Should().BeTrue();
        var courseId = courseResult.Value.CourseId;

        // Act
        var updateResult = await mediator.Send(new UpdateCourseInstructorCommand(courseId, instructorId));

        // Assert
        updateResult.IsSuccess.Should()
            .BeTrue($"because update should succeed, but failed with: {updateResult.TopError.Description}");
        updateResult.Value.InstructorId.Should().Be(instructorId);

        // Verify in database
        using var scope = factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<IAppDbContext>();
        var courseInDb = await dbContext.Courses.FirstOrDefaultAsync(c => c.Id == courseId);
        courseInDb.Should().NotBeNull();
        courseInDb!.InstructorId.Should().Be(instructorId);
    }

    [Fact]
    public async Task Handle_WithNullInstructor_ShouldRemoveInstructor()
    {
        // Arrange
        var mediator = factory.CreateMediator();

        // 1. Create an instructor
        var instructorEmail = $"instructor_{Guid.NewGuid()}@example.com";
        var instructorPhone = $"+1{Guid.NewGuid().ToString()[..10]}";
        var instructorResult = await mediator.Send(new CreateInstructorCommand(
            Name: "Removable Instructor",
            Email: instructorEmail,
            Password: "SecurePass123!",
            PhoneNumber: instructorPhone,
            Role: nameof(UserRole.Instructor)));
        instructorResult.IsSuccess.Should().BeTrue();
        var instructorId = instructorResult.Value.Id;

        // 2. Create a course WITH that instructor
        var courseResult = await mediator.Send(new CreateCourseCommand(
            Name: "Course Remove Instructor",
            InstructorId: instructorId,
            MinimumPassingMarks: 60,
            MaximumMarks: 100));
        courseResult.IsSuccess.Should().BeTrue();
        var courseId = courseResult.Value.CourseId;

        // Act — set instructor to null (unassign)
        var updateResult = await mediator.Send(new UpdateCourseInstructorCommand(courseId, null));

        // Assert
        updateResult.IsSuccess.Should()
            .BeTrue(
                $"because removing instructor should succeed, but failed with: {updateResult.TopError.Description}");
        updateResult.Value.InstructorId.Should().BeNull();

        // Verify in database
        using var scope = factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<IAppDbContext>();
        var courseInDb = await dbContext.Courses.FirstOrDefaultAsync(c => c.Id == courseId);
        courseInDb.Should().NotBeNull();
        courseInDb!.InstructorId.Should().BeNull();
    }

    [Fact]
    public async Task Handle_WithCompletedCourse_ShouldReturnError()
    {
        // Arrange
        var mediator = factory.CreateMediator();

        // 1. Create a course
        var courseResult = await mediator.Send(new CreateCourseCommand(
            Name: "Completed Course",
            InstructorId: null,
            MinimumPassingMarks: 50,
            MaximumMarks: 100));
        courseResult.IsSuccess.Should().BeTrue();
        var courseId = courseResult.Value.CourseId;

        // 2. Mark course as completed directly via DbContext (no application command for this)
        using (var scope = factory.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<IAppDbContext>();
            var course = await dbContext.Courses.FirstOrDefaultAsync(c => c.Id == courseId);
            course.Should().NotBeNull();
            course.MarkAsCompeleted();
            await dbContext.SaveChangesAsync(CancellationToken.None);
        }

        // Act — try to update instructor on a completed course
        var actMediator = factory.CreateMediator();
        var updateResult = await actMediator.Send(new UpdateCourseInstructorCommand(courseId, null));

        // Assert
        updateResult.IsError.Should().BeTrue();
        updateResult.TopError.Code.Should().Be("Course_CannotUpdate_Completed");
    }
}
