using FluentAssertions;

using QuizNova.Application.Features.Courses.Commands.CreateCourse;
using QuizNova.Application.Features.Courses.Queries.GetInstructorCoursesById;
using QuizNova.Application.Features.Instructors.Commands.CreateInstructor;
using QuizNova.Application.Features.Users.DTOs;
using QuizNova.Application.SubcutaneousTests.Common;
using QuizNova.Domain.Entities.Identity;

namespace QuizNova.Application.SubcutaneousTests.Features.Courses.Queries.GetInstructorCoursesById;

public class GetInstructorCoursesByIdQueryHandlerTests(CustomWebApplicationFactory factory)
    : IClassFixture<CustomWebApplicationFactory>
{
    [Fact]
    public async Task Handle_WithEmptyInstructorId_ShouldReturnValidationError()
    {
        // Arrange
        var mediator = factory.CreateMediator();
        var query = new GetInstructorCoursesByIdQuery(Guid.Empty);

        // Act
        var result = await mediator.Send(query);

        // Assert
        result.IsError.Should().BeTrue();
        result.Errors.Should().Contain(e => e.Code == "InstructorId");
    }

    [Fact]
    public async Task Handle_WithNonExistentInstructorId_ShouldReturnSuccessWithEmptyList()
    {
        // Arrange
        var mediator = factory.CreateMediator();
        var query = new GetInstructorCoursesByIdQuery(Guid.NewGuid());

        // Act
        var result = await mediator.Send(query);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_WithValidInstructorIdWithCourses_ShouldReturnCourses()
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
            Name: "Instructor Course Query Test",
            InstructorId: instructorId,
            MinimumPassingMarks: 50,
            MaximumMarks: 100));
        courseResult.IsSuccess.Should().BeTrue();

        // Act
        var result = await mediator.Send(new GetInstructorCoursesByIdQuery(instructorId));

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeEmpty();
        result.Value.Should().Contain(c => c.CourseName == "Instructor Course Query Test");
    }
}
