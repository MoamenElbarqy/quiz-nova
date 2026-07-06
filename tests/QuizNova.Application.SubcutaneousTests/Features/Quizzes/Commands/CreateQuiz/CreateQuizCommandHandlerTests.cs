using System.Net;

using FluentAssertions;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using QuizNova.Application.Common.Errors;
using QuizNova.Application.Features.Quizzes.Commands.CreateQuiz;
using QuizNova.Application.SubcutaneousTests.Common;
using QuizNova.Infrastructure.Data;

namespace QuizNova.Application.SubcutaneousTests.Features.Quizzes.Commands.CreateQuiz;

public class CreateQuizCommandHandlerTests(CustomWebApplicationFactory factory)
    : IClassFixture<CustomWebApplicationFactory>
{
    private readonly List<CreateQuestionCommand> _validQuestions =
    [
        new CreateTfCommand("Q1 Tf", 1, true),
        new CreateTfCommand("Q2 Tf", 1, false),
        new CreateTfCommand("Q3 Tf", 1, true),
    ];

    // --- Validation layer tests ---
    [Fact]
    public async Task Handle_WithEmptyTitle_ShouldReturnValidationError()
    {
        var mediator = factory.CreateMediator();
        var command = new CreateQuizCommand(string.Empty, Guid.NewGuid(), Guid.NewGuid(),
            DateTimeOffset.UtcNow.AddMinutes(10), DateTimeOffset.UtcNow.AddMinutes(30), _validQuestions);

        var result = await mediator.Send(command);

        result.IsError.Should().BeTrue();
        result.Errors.Should().Contain(e => e.Code == "Title");
    }

    [Fact]
    public async Task Handle_WithTitleTooShort_ShouldReturnValidationError()
    {
        var mediator = factory.CreateMediator();
        var command = new CreateQuizCommand("ab", Guid.NewGuid(), Guid.NewGuid(),
            DateTimeOffset.UtcNow.AddMinutes(10), DateTimeOffset.UtcNow.AddMinutes(30), _validQuestions);

        var result = await mediator.Send(command);

        result.IsError.Should().BeTrue();
        result.Errors.Should().Contain(e => e.Code == "Title");
    }

    [Fact]
    public async Task Handle_WithTitleTooLong_ShouldReturnValidationError()
    {
        var mediator = factory.CreateMediator();
        var command = new CreateQuizCommand(new string('a', 31), Guid.NewGuid(), Guid.NewGuid(),
            DateTimeOffset.UtcNow.AddMinutes(10), DateTimeOffset.UtcNow.AddMinutes(30), _validQuestions);

        var result = await mediator.Send(command);

        result.IsError.Should().BeTrue();
        result.Errors.Should().Contain(e => e.Code == "Title");
    }

    [Fact]
    public async Task Handle_WithEmptyCourseId_ShouldReturnValidationError()
    {
        var mediator = factory.CreateMediator();
        var command = new CreateQuizCommand("Valid Title", Guid.Empty, Guid.NewGuid(),
            DateTimeOffset.UtcNow.AddMinutes(10), DateTimeOffset.UtcNow.AddMinutes(30), _validQuestions);

        var result = await mediator.Send(command);

        result.IsError.Should().BeTrue();
        result.Errors.Should().Contain(e => e.Code == "CourseId");
    }

    [Fact]
    public async Task Handle_WithEmptyInstructorId_ShouldReturnValidationError()
    {
        var mediator = factory.CreateMediator();
        var command = new CreateQuizCommand("Valid Title", Guid.NewGuid(), Guid.Empty,
            DateTimeOffset.UtcNow.AddMinutes(10), DateTimeOffset.UtcNow.AddMinutes(30), _validQuestions);

        var result = await mediator.Send(command);

        result.IsError.Should().BeTrue();
        result.Errors.Should().Contain(e => e.Code == "InstructorId");
    }

    [Fact]
    public async Task Handle_WithStartsAtUtcInPast_ShouldReturnValidationError()
    {
        var mediator = factory.CreateMediator();
        var command = new CreateQuizCommand("Valid Title", Guid.NewGuid(), Guid.NewGuid(),
            DateTimeOffset.UtcNow.AddMinutes(-10), DateTimeOffset.UtcNow.AddMinutes(30), _validQuestions);

        var result = await mediator.Send(command);

        result.IsError.Should().BeTrue();
        result.Errors.Should().Contain(e => e.Code == "StartsAtUtc");
    }

    [Fact]
    public async Task Handle_WithEndsAtUtcBeforeStartsAtUtc_ShouldReturnValidationError()
    {
        var mediator = factory.CreateMediator();
        var startsAt = DateTimeOffset.UtcNow.AddMinutes(10);
        var command = new CreateQuizCommand("Valid Title", Guid.NewGuid(), Guid.NewGuid(),
            startsAt, startsAt.AddMinutes(-5), _validQuestions);

        var result = await mediator.Send(command);

        result.IsError.Should().BeTrue();
        result.Errors.Should().Contain(e => e.Code == "EndsAtUtc");
    }

    [Fact]
    public async Task Handle_WithDurationLessThan10Minutes_ShouldReturnValidationError()
    {
        var mediator = factory.CreateMediator();
        var startsAt = DateTimeOffset.UtcNow.AddMinutes(10);
        var command = new CreateQuizCommand("Valid Title", Guid.NewGuid(), Guid.NewGuid(),
            startsAt, startsAt.AddMinutes(5), _validQuestions);

        var result = await mediator.Send(command);

        result.IsError.Should().BeTrue();
        result.Errors.Should().Contain(e => e.Code == "EndsAtUtc");
    }

    [Fact]
    public async Task Handle_WithEmptyQuestionsList_ShouldReturnValidationError()
    {
        var mediator = factory.CreateMediator();
        var command = new CreateQuizCommand("Valid Title", Guid.NewGuid(), Guid.NewGuid(),
            DateTimeOffset.UtcNow.AddMinutes(10), DateTimeOffset.UtcNow.AddMinutes(30),
            new List<CreateQuestionCommand>());

        var result = await mediator.Send(command);

        result.IsError.Should().BeTrue();
        result.Errors.Should().Contain(e => e.Code == "Questions");
    }

    // --- Domain Rules / Handler level tests ---
    [Fact]
    public async Task Handle_WithZeroQuestions_ShouldReturnDomainError()
    {
        var mediator = factory.CreateMediator();

        Guid courseId;
        Guid instructorId;
        using (var scope = factory.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var course = await dbContext.Courses.FirstAsync();
            courseId = course.Id;
            instructorId = course.InstructorId!.Value;
        }

        var command = new CreateQuizCommand("Valid Title", courseId, instructorId,
            DateTimeOffset.UtcNow.AddMinutes(10), DateTimeOffset.UtcNow.AddMinutes(30),
            []);

        var result = await mediator.Send(command);

        result.IsError.Should().BeTrue();
        result.Errors.Should().Contain(e => e.Code == "Questions");
    }

    [Fact]
    public async Task Handle_WithNonExistentCourse_ShouldReturnCourseNotFoundError()
    {
        var mediator = factory.CreateMediator();

        Guid instructorId;
        using (var scope = factory.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var instructor = await dbContext.Instructors.FirstAsync();
            instructorId = instructor.Id;
        }

        var command = new CreateQuizCommand("Valid Title", Guid.NewGuid(), instructorId,
            DateTimeOffset.UtcNow.AddMinutes(10), DateTimeOffset.UtcNow.AddMinutes(30), _validQuestions);

        var result = await mediator.Send(command);

        result.IsError.Should().BeTrue();
        result.TopError.Code.Should().Be(ApplicationErrors.QuizCourseNotFound(command.CourseId).Code);
    }

    [Fact]
    public async Task Handle_WithNonExistentInstructor_ShouldReturnInstructorNotFoundError()
    {
        var mediator = factory.CreateMediator();

        Guid courseId;
        using (var scope = factory.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var course = await dbContext.Courses.FirstAsync();
            courseId = course.Id;
        }

        var command = new CreateQuizCommand("Valid Title", courseId, Guid.NewGuid(),
            DateTimeOffset.UtcNow.AddMinutes(10), DateTimeOffset.UtcNow.AddMinutes(30), _validQuestions);

        var result = await mediator.Send(command);

        result.IsError.Should().BeTrue();
        result.TopError.Code.Should().Be(ApplicationErrors.QuizInstructorNotFound(command.InstructorId).Code);
    }

    [Fact]
    public async Task Handle_WithInstructorNotAssignedToCourse_ShouldReturnNotAssignedError()
    {
        var mediator = factory.CreateMediator();

        Guid courseId;
        Guid wrongInstructorId;
        using (var scope = factory.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var course = await dbContext.Courses.FirstAsync();
            courseId = course.Id;

            // Get an instructor that is NOT the instructor for this course
            var wrongInstructor = await dbContext.Instructors
                .FirstAsync(i => i.Id != course.InstructorId);
            wrongInstructorId = wrongInstructor.Id;
        }

        var command = new CreateQuizCommand("Valid Title", courseId, wrongInstructorId,
            DateTimeOffset.UtcNow.AddMinutes(10), DateTimeOffset.UtcNow.AddMinutes(30), _validQuestions);

        var result = await mediator.Send(command);

        result.IsError.Should().BeTrue();
        result.TopError.Code.Should().Be(
            ApplicationErrors.QuizInstructorIsNotAssignedToCourse(wrongInstructorId, courseId).Code);
    }

    [Fact]
    public async Task Handle_WithValidData_ShouldReturnSuccessAndStoreInDb()
    {
        var mediator = factory.CreateMediator();

        Guid courseId;
        Guid instructorId;
        using (var scope = factory.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var course = await dbContext.Courses.FirstAsync();
            courseId = course.Id;
            instructorId = course.InstructorId!.Value;
        }

        var command = new CreateQuizCommand("Brand New Quiz", courseId, instructorId,
            DateTimeOffset.UtcNow.AddMinutes(10), DateTimeOffset.UtcNow.AddMinutes(30), _validQuestions);

        var result = await mediator.Send(command);

        result.IsSuccess.Should().BeTrue();
        result.Value.Title.Should().Be("Brand New Quiz");
        result.Value.CourseId.Should().Be(courseId);
        result.Value.InstructorId.Should().Be(instructorId);

        // Verify DB State
        using (var scope = factory.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var quiz = await dbContext.Quizzes
                .Include(q => q.Questions)
                .FirstOrDefaultAsync(q => q.Id == result.Value.QuizId);

            quiz.Should().NotBeNull();
            quiz.Title.Should().Be("Brand New Quiz");
            quiz.Questions.Should().HaveCount(3);
        }
    }

    // --- Authorization testing via HTTP pipeline ---
    [Fact]
    public async Task CreateQuiz_AsAdmin_ShouldReturnForbidden()
    {
        // Arrange
        using var client = factory.CreateAppHttpClient();
        await client.AuthenticateAsync("admin@quiznova.local", "Admin123!", "Admin");

        var command = new CreateQuizCommand("Valid Title", Guid.NewGuid(), Guid.NewGuid(),
            DateTimeOffset.UtcNow.AddMinutes(10), DateTimeOffset.UtcNow.AddMinutes(30), _validQuestions);

        // Act
        var response = await client.PostAsJsonAsync("/quizzes", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task CreateQuiz_AsStudent_ShouldReturnForbidden()
    {
        // Arrange
        using var client = factory.CreateAppHttpClient();
        await client.AuthenticateAsync("student1@quiznova.local", "Student123!", "Student");

        var command = new CreateQuizCommand("Valid Title", Guid.NewGuid(), Guid.NewGuid(),
            DateTimeOffset.UtcNow.AddMinutes(10), DateTimeOffset.UtcNow.AddMinutes(30), _validQuestions);

        // Act
        var response = await client.PostAsJsonAsync("/quizzes", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}
