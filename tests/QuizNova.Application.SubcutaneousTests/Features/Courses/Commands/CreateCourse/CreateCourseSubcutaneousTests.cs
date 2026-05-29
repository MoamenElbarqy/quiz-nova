using FluentAssertions;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using QuizNova.Api.SubcutaneousTests.Common;
using QuizNova.Application.Common.Interfaces;
using QuizNova.Application.Features.Courses.Commands.CreateCourse;

namespace QuizNova.Application.SubcutaneousTests.Features.Courses.Commands.CreateCourse;

public class CreateCourseSubcutaneousTests(CustomWebApplicationFactory factory)
    : IClassFixture<CustomWebApplicationFactory>
{
    [Fact]
    public async Task CreateCourse_WithValidDataAndSeededInstructor_ShouldSaveToDatabaseAndReturnSuccess()
    {
        // Arrange
        var mediator = factory.CreateMediator();
        
        // Resolve dbContext to dynamically retrieve the seeded instructor
        using var scope = factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<IAppDbContext>();
        var instructor = await dbContext.Instructors.FirstAsync();
        var instructorId = instructor.Id;
        
        var command = new CreateCourseCommand(
            Name: "Subcutaneous Testing",
            InstructorId: instructorId,
            MinimumPassingMarks: 50,
            MaximumMarks: 100);

        // Act
        // Send the command directly down the pipeline
        var result = await mediator.Send(command);

        // Assert
        result.IsSuccess.Should().BeTrue($"because course creation should succeed but failed with error code: {result.TopError.Code} and message: {result.TopError.Description}");
        result.Value.Should().NotBeNull();
        result.Value.CourseName.Should().Be("Subcutaneous Testing");
        result.Value.InstructorId.Should().Be(instructorId);
    }

    [Fact]
    public async Task CreateCourse_WithNonExistentInstructor_ShouldReturnInstructorNotFoundError()
    {
        // Arrange
        var mediator = factory.CreateMediator();
        var nonExistentInstructorId = Guid.NewGuid();
        
        var command = new CreateCourseCommand(
            Name: "Should Fail Course",
            InstructorId: nonExistentInstructorId,
            MinimumPassingMarks: 50,
            MaximumMarks: 100);

        // Act
        var result = await mediator.Send(command);

        // Assert
        result.IsError.Should().BeTrue();
        result.TopError.Code.Should().Be("Instructor_NotFound");
    }
}
