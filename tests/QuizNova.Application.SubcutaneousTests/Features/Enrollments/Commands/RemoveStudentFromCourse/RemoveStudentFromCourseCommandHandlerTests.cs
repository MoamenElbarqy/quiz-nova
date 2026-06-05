using FluentAssertions;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using QuizNova.Application.Common.Errors;
using QuizNova.Application.Common.Interfaces;
using QuizNova.Application.Features.Courses.Commands.CreateCourse;
using QuizNova.Application.Features.Enrollments.Commands.EnrollStudentInCourse;
using QuizNova.Application.Features.Enrollments.Commands.RemoveStudentFromCourse;
using QuizNova.Application.Features.Students.Commands.CreateStudent;
using QuizNova.Application.SubcutaneousTests.Common;
using QuizNova.Domain.Entities.Identity;

namespace QuizNova.Application.SubcutaneousTests.Features.Enrollments.Commands.RemoveStudentFromCourse;

public class RemoveStudentFromCourseCommandHandlerTests(CustomWebApplicationFactory factory)
    : IClassFixture<CustomWebApplicationFactory>
{
    [Fact]
    public async Task Handle_WithEmptyEnrollmentId_ShouldReturnValidationError()
    {
        // Arrange
        var mediator = factory.CreateMediator();
        var command = new RemoveStudentFromCourseCommand(Guid.Empty, Guid.NewGuid());

        // Act
        var result = await mediator.Send(command);

        // Assert
        result.IsError.Should().BeTrue();
        result.Errors.Should().Contain(e => e.Code == "EnrollmentId");
    }

    [Fact]
    public async Task Handle_WithEmptyStudentId_ShouldReturnValidationError()
    {
        // Arrange
        var mediator = factory.CreateMediator();
        var command = new RemoveStudentFromCourseCommand(Guid.NewGuid(), Guid.Empty);

        // Act
        var result = await mediator.Send(command);

        // Assert
        result.IsError.Should().BeTrue();
        result.Errors.Should().Contain(e => e.Code == "StudentId");
    }

    [Fact]
    public async Task Handle_WithNonExistentEnrollment_ShouldReturnNotFoundError()
    {
        // Arrange
        var mediator = factory.CreateMediator();
        var command = new RemoveStudentFromCourseCommand(Guid.NewGuid(), Guid.NewGuid());

        // Act
        var result = await mediator.Send(command);

        // Assert
        result.IsError.Should().BeTrue();
        result.TopError.Code.Should().Be(ApplicationErrors.EnrollmentNotFound(Guid.Empty).Code);
    }

    [Fact]
    public async Task Handle_WithValidEnrollment_ShouldRemoveSuccessfully()
    {
        // Arrange
        var mediator = factory.CreateMediator();

        // 1. Create a student
        var studentEmail = $"student_{Guid.NewGuid()}@example.com";
        var studentPhone = $"+1{Guid.NewGuid().ToString()[..10]}";
        var studentResult = await mediator.Send(new CreateStudentCommand(
            Name: "Remove Student",
            Email: studentEmail,
            Password: "SecurePass123!",
            PhoneNumber: studentPhone,
            Role: nameof(UserRole.Student)));
        studentResult.IsSuccess.Should().BeTrue();
        var studentId = studentResult.Value.Id;

        // 2. Create a course
        var courseResult = await mediator.Send(new CreateCourseCommand(
            Name: "Course For Remove",
            InstructorId: null,
            MinimumPassingMarks: 50,
            MaximumMarks: 100));
        courseResult.IsSuccess.Should().BeTrue();
        var courseId = courseResult.Value.Id;

        // 3. Enroll the student
        var enrollResult = await mediator.Send(new EnrollStudentInCourseCommand(courseId, studentId));
        enrollResult.IsSuccess.Should().BeTrue();

        // 4. Retrieve the enrollment ID from the database
        Guid enrollmentId;
        using (var scope = factory.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<IAppDbContext>();
            var enrollment = await dbContext.Enrollments
                .FirstOrDefaultAsync(e => e.StudentId == studentId && e.CourseId == courseId);
            enrollment.Should().NotBeNull();
            enrollmentId = enrollment.Id;
        }

        // Act
        var removeResult = await mediator.Send(new RemoveStudentFromCourseCommand(enrollmentId, studentId));

        // Assert
        removeResult.IsSuccess.Should()
            .BeTrue($"because removal should succeed, but failed with: {removeResult.TopError.Description}");

        // Verify enrollment is gone from database
        using var verifyScope = factory.Services.CreateScope();
        var verifyDb = verifyScope.ServiceProvider.GetRequiredService<IAppDbContext>();
        var enrollmentInDb = await verifyDb.Enrollments
            .FirstOrDefaultAsync(e => e.Id == enrollmentId);
        enrollmentInDb.Should().BeNull();
    }

    [Fact]
    public async Task Handle_WithAlreadyRemovedEnrollment_ShouldReturnNotFoundError()
    {
        // Arrange
        var mediator = factory.CreateMediator();

        // 1. Create a student
        var studentEmail = $"student_{Guid.NewGuid()}@example.com";
        var studentPhone = $"+1{Guid.NewGuid().ToString()[..10]}";
        var studentResult = await mediator.Send(new CreateStudentCommand(
            Name: "Double Remove Student",
            Email: studentEmail,
            Password: "SecurePass123!",
            PhoneNumber: studentPhone,
            Role: nameof(UserRole.Student)));
        studentResult.IsSuccess.Should().BeTrue();
        var studentId = studentResult.Value.Id;

        // 2. Create a course
        var courseResult = await mediator.Send(new CreateCourseCommand(
            Name: "Course Double Remove",
            InstructorId: null,
            MinimumPassingMarks: 50,
            MaximumMarks: 100));
        courseResult.IsSuccess.Should().BeTrue();
        var courseId = courseResult.Value.Id;

        // 3. Enroll the student
        var enrollResult = await mediator.Send(new EnrollStudentInCourseCommand(courseId, studentId));
        enrollResult.IsSuccess.Should().BeTrue();

        // 4. Retrieve the enrollment ID
        Guid enrollmentId;
        using (var scope = factory.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<IAppDbContext>();
            var enrollment = await dbContext.Enrollments
                .FirstOrDefaultAsync(e => e.StudentId == studentId && e.CourseId == courseId);
            enrollment.Should().NotBeNull();
            enrollmentId = enrollment!.Id;
        }

        var removeCommand = new RemoveStudentFromCourseCommand(enrollmentId, studentId);

        // Act
        var removeResult1 = await mediator.Send(removeCommand);
        var removeResult2 = await mediator.Send(removeCommand);

        // Assert
        removeResult1.IsSuccess.Should().BeTrue();
        removeResult2.IsError.Should().BeTrue();
        removeResult2.TopError.Code.Should().Be(ApplicationErrors.EnrollmentNotFound(Guid.Empty).Code);

        // Verify enrollment is still absent in database
        using var verifyScope = factory.Services.CreateScope();
        var verifyDb = verifyScope.ServiceProvider.GetRequiredService<IAppDbContext>();
        var enrollmentInDb = await verifyDb.Enrollments
            .FirstOrDefaultAsync(e => e.Id == enrollmentId);
        enrollmentInDb.Should().BeNull();
    }
}
