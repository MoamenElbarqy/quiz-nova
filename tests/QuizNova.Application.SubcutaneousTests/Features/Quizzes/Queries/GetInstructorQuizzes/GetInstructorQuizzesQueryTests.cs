using FluentAssertions;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using QuizNova.Application.Features.Quizzes.Commands.CreateQuiz;
using QuizNova.Application.Features.Quizzes.Queries.GetInstructorQuizzes;
using QuizNova.Application.SubcutaneousTests.Common;
using QuizNova.Infrastructure.Data;

namespace QuizNova.Application.SubcutaneousTests.Features.Quizzes.Queries.GetInstructorQuizzes;

public class GetInstructorQuizzesQueryTests(CustomWebApplicationFactory factory)
    : IClassFixture<CustomWebApplicationFactory>
{
    [Fact]
    public async Task Handle_WithEmptyInstructorId_ShouldReturnValidationError()
    {
        var mediator = factory.CreateMediator();
        var query = new GetInstructorQuizzesQuery(Guid.Empty);

        var result = await mediator.Send(query);

        result.IsError.Should().BeTrue();
        result.Errors.Should().Contain(e => e.Code == "InstructorId");
    }

    [Fact]
    public async Task Handle_WithNonExistentInstructorId_ShouldReturnNotFoundError()
    {
        var mediator = factory.CreateMediator();
        var query = new GetInstructorQuizzesQuery(Guid.NewGuid());

        var result = await mediator.Send(query);

        result.IsError.Should().BeTrue();
        result.Errors.Should().Contain(e => e.Code == "Instructor_NotFound");
    }

    [Fact]
    public async Task Handle_WithValidInstructorAndQuizzes_ShouldReturnInstructorQuizzes()
    {
        var mediator = factory.CreateMediator();

        Guid instructorId;
        Guid courseId;
        using (var scope = factory.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var course = await dbContext.Courses.FirstAsync();
            courseId = course.Id;
            instructorId = course.InstructorId!.Value;
        }

        var questions = new List<CreateQuestionCommand>
        {
            new CreateTfCommand("Question 1", 1, true),
            new CreateTfCommand("Question 2", 2, false),
            new CreateTfCommand("Question 3", 3, true),
        };
        var quizTitle = $"Quiz {Guid.NewGuid().ToString()[..8]}";
        var quizResult = await mediator.Send(new CreateQuizCommand(quizTitle, courseId, instructorId,
            DateTimeOffset.UtcNow.AddDays(1), DateTimeOffset.UtcNow.AddDays(2), questions));
        quizResult.IsSuccess.Should().BeTrue();

        var query = new GetInstructorQuizzesQuery(instructorId);

        var result = await mediator.Send(query);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Should().Contain(q => q.Title == quizTitle);
    }
}
