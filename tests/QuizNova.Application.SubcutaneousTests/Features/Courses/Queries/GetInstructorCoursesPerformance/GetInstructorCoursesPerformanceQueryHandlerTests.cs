using FluentAssertions;

using QuizNova.Application.Features.Courses.Commands.CreateCourse;
using QuizNova.Application.Features.Courses.Queries.GetInstructorCoursesPerformance;
using QuizNova.Application.Features.Instructors.Commands.CreateInstructor;
using QuizNova.Application.Features.Users.DTOs;
using QuizNova.Application.SubcutaneousTests.Common;
using QuizNova.Domain.Entities.Identity;

namespace QuizNova.Application.SubcutaneousTests.Features.Courses.Queries.GetInstructorCoursesPerformance;

public class GetInstructorCoursesPerformanceQueryHandlerTests(CustomWebApplicationFactory factory)
    : IClassFixture<CustomWebApplicationFactory>
{
    [Fact]
    public async Task Handle_WithEmptyInstructorId_ShouldReturnValidationError()
    {
        // Arrange
        var mediator = factory.CreateMediator();
        var query = new GetInstructorCoursesPerformanceQuery(Guid.Empty);

        // Act
        var result = await mediator.Send(query);

        // Assert
        result.IsError.Should().BeTrue();
        result.Errors.Should().Contain(e => e.Code == "InstructorId");
    }

    [Fact]
    public async Task Handle_WithNonExistentInstructorId_ShouldReturnNotFoundError()
    {
        // Arrange
        var mediator = factory.CreateMediator();
        var query = new GetInstructorCoursesPerformanceQuery(Guid.NewGuid());

        // Act
        var result = await mediator.Send(query);

        // Assert
        result.IsError.Should().BeTrue();
        result.Errors.Should().Contain(e => e.Code == "Instructor_NotFound");
    }

    [Fact]
    public async Task Handle_WithValidInstructorId_ShouldReturnPerformanceData()
    {
        // Arrange
        var mediator = factory.CreateMediator();

        // Create an instructor
        var email = $"inst_{Guid.NewGuid()}@test.com";
        var phone = $"+1{Guid.NewGuid().ToString()[..10]}";
        var instructorResult = await mediator.Send(new CreateInstructorCommand(
            PersonalInformation: new PersonalInformationDto("Test Instructor", email, phone),
            Password: "SecurePass1!",
            Role: nameof(UserRole.Instructor)));
        instructorResult.IsSuccess.Should().BeTrue();
        var instructorId = instructorResult.Value.Id;

        // Create a course assigned to this instructor
        var courseResult = await mediator.Send(new CreateCourseCommand(
            Name: "Performance Query Course Test",
            InstructorId: instructorId,
            MinimumPassingMarks: 50,
            MaximumMarks: 100));
        courseResult.IsSuccess.Should().BeTrue();

        // Act
        var result = await mediator.Send(new GetInstructorCoursesPerformanceQuery(instructorId));

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeEmpty();
        var coursePerf = result.Value.Find(c => c.Name == "Performance Query Course Test");
        coursePerf.Should().NotBeNull();
        coursePerf.NumberOfStudents.Should().Be(0);
        coursePerf.AvgScore.Should().Be(0.0);
    }
}
