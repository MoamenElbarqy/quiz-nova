using FluentAssertions;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using QuizNova.Application.Common.Errors;
using QuizNova.Application.Features.Quizzes.Commands.UpdateQuizMetadata;
using QuizNova.Application.SubcutaneousTests.Common;
using QuizNova.Domain.Entities.Quizzes;
using QuizNova.Domain.Entities.Quizzes.Questions.Base;
using QuizNova.Infrastructure.Data;

namespace QuizNova.Application.SubcutaneousTests.Features.Quizzes.Commands.UpdateQuizMetadata;

public class UpdateQuizMetadataCommandHandlerTests(CustomWebApplicationFactory factory)
    : IClassFixture<CustomWebApplicationFactory>
{
    // --- Validation tests ---
    [Fact]
    public async Task Handle_WithEmptyQuizId_ShouldReturnValidationError()
    {
        var mediator = factory.CreateMediator();
        var command = new UpdateQuizMetadataCommand(Guid.Empty, "Valid Title", DateTimeOffset.UtcNow.AddMinutes(10),
            DateTimeOffset.UtcNow.AddMinutes(30));

        var result = await mediator.Send(command);

        result.IsError.Should().BeTrue();
        result.Errors.Should().Contain(e => e.Code == "QuizId");
    }

    [Fact]
    public async Task Handle_WithEmptyTitle_ShouldReturnValidationError()
    {
        var mediator = factory.CreateMediator();
        var command = new UpdateQuizMetadataCommand(Guid.NewGuid(), string.Empty, DateTimeOffset.UtcNow.AddMinutes(10),
            DateTimeOffset.UtcNow.AddMinutes(30));

        var result = await mediator.Send(command);

        result.IsError.Should().BeTrue();
        result.Errors.Should().Contain(e => e.Code == "Title");
    }

    [Fact]
    public async Task Handle_WithTitleTooShort_ShouldReturnValidationError()
    {
        var mediator = factory.CreateMediator();
        var command = new UpdateQuizMetadataCommand(Guid.NewGuid(), "ab", DateTimeOffset.UtcNow.AddMinutes(10),
            DateTimeOffset.UtcNow.AddMinutes(30));

        var result = await mediator.Send(command);

        result.IsError.Should().BeTrue();
        result.Errors.Should().Contain(e => e.Code == "Title");
    }

    [Fact]
    public async Task Handle_WithTitleTooLong_ShouldReturnValidationError()
    {
        var mediator = factory.CreateMediator();
        var command = new UpdateQuizMetadataCommand(Guid.NewGuid(), new string('a', 31),
            DateTimeOffset.UtcNow.AddMinutes(10), DateTimeOffset.UtcNow.AddMinutes(30));

        var result = await mediator.Send(command);

        result.IsError.Should().BeTrue();
        result.Errors.Should().Contain(e => e.Code == "Title");
    }

    [Fact]
    public async Task Handle_WithEndsAtUtcBeforeStartsAtUtc_ShouldReturnValidationError()
    {
        var mediator = factory.CreateMediator();
        var startsAt = DateTimeOffset.UtcNow.AddMinutes(10);
        var command = new UpdateQuizMetadataCommand(Guid.NewGuid(), "Valid Title", startsAt, startsAt.AddMinutes(-5));

        var result = await mediator.Send(command);

        result.IsError.Should().BeTrue();
        result.Errors.Should().Contain(e => e.Code == "EndsAtUtc");
    }

    // --- Domain tests ---
    [Fact]
    public async Task Handle_WithNonExistentQuiz_ShouldReturnQuizNotFoundError()
    {
        var mediator = factory.CreateMediator();
        var command = new UpdateQuizMetadataCommand(Guid.NewGuid(), "Valid Title", DateTimeOffset.UtcNow.AddMinutes(10),
            DateTimeOffset.UtcNow.AddMinutes(30));

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

            // Get a quiz that is already started (StartsAtUtc is in the past)
            // DbInitializer sets startsAt to -30 mins and endsAt to +3 hours, making it ActiveNow
            var quiz = await dbContext.Quizzes.FirstAsync(q => q.StartsAtUtc <= fakeTime.GetUtcNow() && q.EndsAtUtc >= fakeTime.GetUtcNow());
            quizId = quiz.Id;
        }

        var command = new UpdateQuizMetadataCommand(quizId, "Updated Title", DateTimeOffset.UtcNow.AddMinutes(10),
            DateTimeOffset.UtcNow.AddMinutes(30));

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

        // Advance time to after the quiz ends (DbInitializer sets it to +3 hours)
        fakeTime.Advance(TimeSpan.FromHours(4));

        var command = new UpdateQuizMetadataCommand(quizId, "Updated Title", DateTimeOffset.UtcNow.AddMinutes(10),
            DateTimeOffset.UtcNow.AddMinutes(30));

        var result = await mediator.Send(command);

        result.IsError.Should().BeTrue();
        result.TopError.Code.Should().Be("Quiz.CannotUpdateStartedOrCompletedQuiz");
    }

    [Fact]
    public async Task Handle_WithDurationLessThan10Minutes_ShouldReturnScheduleDurationError()
    {
        var mediator = factory.CreateMediator();
        var fakeTime = factory.GetFakeTimeProvider();

        Guid quizId;
        using (var scope = factory.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            // Find a course that isn't completed
            var course = await dbContext.Courses.FirstAsync();

            // Create a scheduled quiz explicitly (future start date)
            var quiz = Quiz.Create(Guid.NewGuid(), course.Id, course.InstructorId!.Value, "Future Quiz",
                fakeTime.GetUtcNow().AddDays(1), fakeTime.GetUtcNow().AddDays(1).AddHours(1), new List<Question>()).Value;

            await dbContext.Quizzes.AddAsync(quiz);
            await dbContext.SaveChangesAsync();
            quizId = quiz.Id;
        }

        var startsAt = fakeTime.GetUtcNow().AddDays(2);
        var command = new UpdateQuizMetadataCommand(quizId, "Updated Title", startsAt, startsAt.AddMinutes(5)); // < 10 mins

        var result = await mediator.Send(command);

        result.IsError.Should().BeTrue();
        result.TopError.Code.Should().Be("Quiz.ScheduleDurationTooShort");
    }

    [Fact]
    public async Task Handle_WithValidDataOnScheduledQuiz_ShouldReturnSuccessAndStoreInDb()
    {
        var mediator = factory.CreateMediator();
        var fakeTime = factory.GetFakeTimeProvider();

        Guid quizId;
        using (var scope = factory.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            // Find a course that isn't completed
            var course = await dbContext.Courses.FirstAsync();

            // Create a scheduled quiz explicitly
            var quiz = Quiz.Create(Guid.NewGuid(), course.Id, course.InstructorId!.Value, "Future Quiz",
                fakeTime.GetUtcNow().AddDays(1), fakeTime.GetUtcNow().AddDays(1).AddHours(1), new List<Question>()).Value;

            await dbContext.Quizzes.AddAsync(quiz);
            await dbContext.SaveChangesAsync();
            quizId = quiz.Id;
        }

        var newStart = fakeTime.GetUtcNow().AddDays(2);
        var newEnd = newStart.AddHours(2);
        var command = new UpdateQuizMetadataCommand(quizId, "Awesome Updated Title", newStart, newEnd);

        var result = await mediator.Send(command);

        result.IsSuccess.Should().BeTrue();

        // Verify DB State
        using (var scope = factory.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var quiz = await dbContext.Quizzes.FirstAsync(q => q.Id == quizId);

            quiz.Title.Should().Be("Awesome Updated Title");
            quiz.StartsAtUtc.Should().Be(newStart);
            quiz.EndsAtUtc.Should().Be(newEnd);
        }
    }
}
