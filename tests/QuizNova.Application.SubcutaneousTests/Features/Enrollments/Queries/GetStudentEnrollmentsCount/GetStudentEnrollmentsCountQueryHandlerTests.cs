using FluentAssertions;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using QuizNova.Application.Features.Enrollments.Queries.GetStudentEnrollmentsCount;
using QuizNova.Application.SubcutaneousTests.Common;
using QuizNova.Infrastructure.Data;

namespace QuizNova.Application.SubcutaneousTests.Features.Enrollments.Queries.GetStudentEnrollmentsCount;

public class GetStudentEnrollmentsCountQueryHandlerTests(CustomWebApplicationFactory factory)
    : IClassFixture<CustomWebApplicationFactory>
{
    [Fact]
    public async Task Handle_WithEmptyStudentId_ShouldReturnValidationError()
    {
        // Arrange
        var mediator = factory.CreateMediator();
        var query = new GetStudentEnrollmentsCountQuery(Guid.Empty);

        // Act
        var result = await mediator.Send(query);

        // Assert
        result.IsError.Should().BeTrue();
        result.Errors.Should().Contain(e => e.Code == "StudentId");
    }

    [Fact]
    public async Task Handle_WithNonExistentStudentId_ShouldReturnNotFoundError()
    {
        // Arrange
        var mediator = factory.CreateMediator();
        var query = new GetStudentEnrollmentsCountQuery(Guid.NewGuid());

        // Act
        var result = await mediator.Send(query);

        // Assert
        result.IsError.Should().BeTrue();
        result.Errors.Should().Contain(e => e.Code == "Student.NotFound");
    }

    [Fact]
    public async Task Handle_WithValidStudentIdWithEnrollments_ShouldReturnCorrectCount()
    {
        // Arrange
        var mediator = factory.CreateMediator();

        Guid studentId;
        int initialCount;
        using (var scope = factory.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var student = await dbContext.Students.FirstAsync();
            studentId = student.Id;
            initialCount = await dbContext.Enrollments.CountAsync(e => e.StudentId == studentId);
        }

        // We use the initial count since DbInitializer already adds enrollments

        // Act
        var result = await mediator.Send(new GetStudentEnrollmentsCountQuery(studentId));

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.EnrollmentsCount.Should().Be(initialCount);
    }
}
