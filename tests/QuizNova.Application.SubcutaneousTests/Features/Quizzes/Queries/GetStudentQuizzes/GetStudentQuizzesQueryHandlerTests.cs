using FluentAssertions;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using QuizNova.Application.Features.Enrollments.Commands.EnrollStudentInCourse;
using QuizNova.Application.Features.Quizzes.Commands.CreateQuiz;
using QuizNova.Application.Features.Quizzes.Queries.GetStudentQuizzes;
using QuizNova.Application.SubcutaneousTests.Common;
using QuizNova.Infrastructure.Data;

namespace QuizNova.Application.SubcutaneousTests.Features.Quizzes.Queries.GetStudentQuizzes;

public class GetStudentQuizzesQueryHandlerTests(CustomWebApplicationFactory factory)
    : IClassFixture<CustomWebApplicationFactory>
{
    [Fact]
    public async Task Handle_WithEmptyStudentId_ShouldReturnValidationError()
    {
        var mediator = factory.CreateMediator();
        var query = new GetStudentQuizzesQuery(Guid.Empty);

        var result = await mediator.Send(query);

        result.IsError.Should().BeTrue();
        result.Errors.Should().Contain(e => e.Code == "StudentId");
    }

    [Fact]
    public async Task Handle_WithValidStudentButNoEnrollments_ShouldReturnEmptyList()
    {
        var mediator = factory.CreateMediator();
        var query = new GetStudentQuizzesQuery(Guid.NewGuid());

        var result = await mediator.Send(query);

        result.IsSuccess.Should().BeTrue();
        result.Value.Quizzes.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_WithValidStudentAndQuizzes_ShouldReturnStudentQuizzes()
    {
        var mediator = factory.CreateMediator();

        Guid studentId;
        Guid courseId;
        Guid instructorId;
        using (var scope = factory.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            studentId = (await dbContext.Students.FirstAsync()).Id;
            var course = await dbContext.Courses.FirstAsync();
            courseId = course.Id;
            instructorId = course.InstructorId!.Value;
        }

        // Enroll student and create quiz
        await mediator.Send(new EnrollStudentInCourseCommand(studentId, courseId));

        var questions = new List<CreateQuestionCommand>
        {
            new CreateTfCommand("Q1", 1, true),
            new CreateTfCommand("Q2", 1, false),
            new CreateTfCommand("Q3", 1, true),
        };
        var quizTitle = $"Student Quiz {Guid.NewGuid()}";
        await mediator.Send(new CreateQuizCommand(quizTitle, courseId, instructorId,
            DateTimeOffset.UtcNow.AddDays(1), DateTimeOffset.UtcNow.AddDays(1).AddHours(1), questions));

        var query = new GetStudentQuizzesQuery(studentId);

        var result = await mediator.Send(query);

        result.IsSuccess.Should().BeTrue();
        result.Value.Quizzes.Should().NotBeEmpty();
        result.Value.Quizzes.Should().Contain(q => q.Title == quizTitle);
    }
}
