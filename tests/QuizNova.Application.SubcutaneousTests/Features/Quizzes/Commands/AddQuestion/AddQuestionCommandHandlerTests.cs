using FluentAssertions;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using QuizNova.Application.Common.Errors;
using QuizNova.Application.Features.Quizzes.Commands.AddQuestion;
using QuizNova.Application.Features.Quizzes.Commands.CreateQuiz;
using QuizNova.Application.SubcutaneousTests.Common;
using QuizNova.Domain.Entities.Quizzes;
using QuizNova.Domain.Entities.Quizzes.Questions.Base;
using QuizNova.Infrastructure.Data;

namespace QuizNova.Application.SubcutaneousTests.Features.Quizzes.Commands.AddQuestion;

public class AddQuestionCommandHandlerTests(CustomWebApplicationFactory factory)
    : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CreateQuestionCommand _validQuestion = new CreateTfCommand("Valid Tf Question", 1, true);

    // --- Validation tests ---

    [Fact]
    public async Task Handle_WithEmptyQuizId_ShouldReturnValidationError()
    {
        var mediator = factory.CreateMediator();
        var command = new AddQuestionCommand(Guid.Empty, _validQuestion);

        var result = await mediator.Send(command);

        result.IsError.Should().BeTrue();
        result.Errors.Should().Contain(e => e.Code == "QuizId");
    }

    [Fact]
    public async Task Handle_WithInvalidQuestionText_ShouldReturnValidationError()
    {
        var mediator = factory.CreateMediator();
        var invalidQuestion = new CreateTfCommand("ab", 1, true); // < 3 chars
        var command = new AddQuestionCommand(Guid.NewGuid(), invalidQuestion);

        var result = await mediator.Send(command);

        result.IsError.Should().BeTrue();
        result.Errors.Should().Contain(e => e.Code == "Question.QuestionText");
    }

    [Fact]
    public async Task Handle_WithInvalidMarks_ShouldReturnValidationError()
    {
        var mediator = factory.CreateMediator();
        var invalidQuestion = new CreateTfCommand("Valid Question", 6, true); // marks > 5
        var command = new AddQuestionCommand(Guid.NewGuid(), invalidQuestion);

        var result = await mediator.Send(command);

        result.IsError.Should().BeTrue();
        result.Errors.Should().Contain(e => e.Code == "Question.Marks");
    }

    // --- Domain tests ---

    [Fact]
    public async Task Handle_WithNonExistentQuiz_ShouldReturnQuizNotFoundError()
    {
        var mediator = factory.CreateMediator();
        var command = new AddQuestionCommand(Guid.NewGuid(), _validQuestion);

        var result = await mediator.Send(command);

        result.IsError.Should().BeTrue();
        result.TopError.Code.Should().Be(ApplicationErrors.QuizNotFound(command.QuizId).Code);
    }

    [Fact]
    public async Task Handle_WithActiveQuiz_ShouldReturnCannotUpdateError()
    {
        var mediator = factory.CreateMediator();
        var fakeTime = factory.GetFakeTimeProvider();

        Guid quizId;
        using (var scope = factory.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            // Find active quiz
            var quiz = await dbContext.Quizzes.FirstAsync(q => q.StartsAtUtc <= fakeTime.GetUtcNow() && q.EndsAtUtc >= fakeTime.GetUtcNow());
            quizId = quiz.Id;
        }

        var command = new AddQuestionCommand(quizId, _validQuestion);

        var result = await mediator.Send(command);

        result.IsError.Should().BeTrue();
        result.TopError.Code.Should().Be("Quiz.CannotUpdateStartedOrCompletedQuiz");
    }

    [Fact]
    public async Task Handle_WithCompletedQuiz_ShouldReturnCannotUpdateError()
    {
        var mediator = factory.CreateMediator();
        var fakeTime = factory.GetFakeTimeProvider();

        Guid quizId;
        using (var scope = factory.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var quiz = await dbContext.Quizzes.FirstAsync();
            quizId = quiz.Id;
        }

        fakeTime.Advance(TimeSpan.FromHours(4)); // Move past EndsAtUtc

        var command = new AddQuestionCommand(quizId, _validQuestion);

        var result = await mediator.Send(command);

        result.IsError.Should().BeTrue();
        result.TopError.Code.Should().Be("Quiz.CannotUpdateStartedOrCompletedQuiz");
    }

    [Fact]
    public async Task Handle_WithValidDataOnScheduledQuiz_ShouldReturnSuccessAndStoreInDb()
    {
        var mediator = factory.CreateMediator();
        var fakeTime = factory.GetFakeTimeProvider();

        Guid quizId;
        int initialQuestionCount;
        using (var scope = factory.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var course = await dbContext.Courses.FirstAsync();

            var quizResult = Quiz.Create(Guid.NewGuid(), course.Id, course.InstructorId!.Value, "Future Quiz",
                fakeTime.GetUtcNow().AddDays(1), fakeTime.GetUtcNow().AddDays(1).AddHours(1), new List<Question>());

            var quiz = quizResult.Value;

            // Add a single question to bypass the 'Must have at least 3 questions' logic temporarily or handle the fact that
            // it's already instantiated successfully. The Quiz.Create method actually doesn't restrict empty lists in creation
            // unless we enforce it in the handler/validator (which we do in CreateQuizCommandHandler, but here we instantiate directly).

            await dbContext.Quizzes.AddAsync(quiz);
            await dbContext.SaveChangesAsync();
            quizId = quiz.Id;
            initialQuestionCount = quiz.Questions.Count();
        }

        var command = new AddQuestionCommand(quizId, _validQuestion);

        var result = await mediator.Send(command);

        result.IsSuccess.Should().BeTrue();
        result.Value.QuestionText.Should().Be("Valid Tf Question");

        // Verify DB State
        using (var scope = factory.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var quiz = await dbContext.Quizzes.Include(q => q.Questions).FirstAsync(q => q.Id == quizId);

            quiz.Questions.Count().Should().Be(initialQuestionCount + 1);
            quiz.Questions.Should().Contain(q => q.QuestionText == "Valid Tf Question");
        }
    }
}
