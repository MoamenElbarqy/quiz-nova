using System.Net;

using FluentAssertions;

using Microsoft.EntityFrameworkCore;

using MongoDB.Driver;

using QuizNova.Api.IntegrationTests.Common;
using QuizNova.Application.Common.Interfaces;
using QuizNova.Application.Common.Models;
using QuizNova.Application.Features.QuizAttempts.DTOs;
using QuizNova.Domain.Entities.Identity;
using QuizNova.Domain.Entities.QuizAttempts;
using QuizNova.Domain.Entities.Quizzes;
using QuizNova.Domain.Entities.Quizzes.Questions;
using QuizNova.Domain.Entities.Quizzes.Questions.AutoGradedQuestions.TrueFalse;
using QuizNova.Domain.Entities.Quizzes.Questions.Base;
using QuizNova.Tests.Common.Security;

using Xunit;

namespace QuizNova.Api.IntegrationTests.Controllers;

public class QuizAttemptControllerTests(CustomWebApplicationFactory factory)
    : IClassFixture<CustomWebApplicationFactory>
{
    [Fact]
    public async Task GetAllQuizzesAttempts_WhenUnauthenticated_ReturnsUnauthorized()
    {
        using var client = factory.CreateAppHttpClient();

        var response = await client.GetAsync("/quiz-attempts");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetQuizAttemptById_WhenStudent_ReturnsQuizAttemptDto()
    {
        using var client = factory.CreateAppHttpClient();
        await client.AuthenticateAsync(TestUsers.Student.User.Email!, TestUsers.Student.Password, "Student");
        var (attemptId, studentId, _) = await SeedQuizAttemptAsync();

        var response = await client.GetAsync($"/students/{studentId}/quiz-attempts/{attemptId}");

        response.StatusCode.Should().Be(HttpStatusCode.OK, await response.Content.ReadAsStringAsync());

        var attempt = await response.Content.ReadFromJsonAsync<QuizAttemptDto>();
        attempt.Should().NotBeNull();
        attempt.QuizAttemptId.Should().Be(attemptId);
    }

    [Fact]
    public async Task GetQuizAttemptById_WhenAdmin_ReturnsQuizAttemptDto()
    {
        using var client = factory.CreateAppHttpClient();
        await client.AuthenticateAsync(TestUsers.Admin.User.Email!, TestUsers.Admin.Password, "Admin");
        var (attemptId, studentId, _) = await SeedQuizAttemptAsync();

        var response = await client.GetAsync($"/students/{studentId}/quiz-attempts/{attemptId}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetQuizAttemptById_WithNonExistentId_ReturnsNotFound()
    {
        using var client = factory.CreateAppHttpClient();
        await client.AuthenticateAsync(TestUsers.Student.User.Email!, TestUsers.Student.Password, "Student");
        var (_, studentId, _) = await GetSeededIdsAsync();

        var response = await client.GetAsync($"/students/{studentId}/quiz-attempts/{Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetQuizAttemptById_WithEmptyId_ReturnsBadRequest()
    {
        using var client = factory.CreateAppHttpClient();
        await client.AuthenticateAsync(TestUsers.Student.User.Email!, TestUsers.Student.Password, "Student");
        var (_, studentId, _) = await GetSeededIdsAsync();

        var response = await client.GetAsync($"/students/{studentId}/quiz-attempts/{Guid.Empty}");

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetQuizAttemptByIdForGrading_WhenStudent_ReturnsQuizAttemptDto()
    {
        using var client = factory.CreateAppHttpClient();
        await client.AuthenticateAsync(TestUsers.Student.User.Email!, TestUsers.Student.Password, "Student");
        var (attemptId, _, _) = await SeedQuizAttemptAsync();

        var response = await client.GetAsync($"/quiz-attempts/{attemptId}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetQuizAttemptByIdForGrading_WhenAdmin_ReturnsForbidden()
    {
        using var client = factory.CreateAppHttpClient();
        await client.AuthenticateAsync(TestUsers.Admin.User.Email!, TestUsers.Admin.Password, "Admin");
        var (attemptId, _, _) = await SeedQuizAttemptAsync();

        var response = await client.GetAsync($"/quiz-attempts/{attemptId}");

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task GetQuizAttemptByIdForGrading_WhenInstructor_ReturnsQuizAttemptDto()
    {
        using var client = factory.CreateAppHttpClient();
        await client.AuthenticateAsync(TestUsers.Instructor1.User.Email!, TestUsers.Instructor1.Password, "Instructor");
        var (attemptId, _, _) = await SeedQuizAttemptAsync();

        var response = await client.GetAsync($"/quiz-attempts/{attemptId}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var attempt = await response.Content.ReadFromJsonAsync<QuizAttemptDto>();
        attempt.Should().NotBeNull();
        attempt.QuizAttemptId.Should().Be(attemptId);
    }

    [Fact]
    public async Task GetStudentQuizAttempts_WhenAdmin_ReturnsForbidden()
    {
        using var client = factory.CreateAppHttpClient();
        await client.AuthenticateAsync(TestUsers.Admin.User.Email!, TestUsers.Admin.Password, "Admin");
        var (_, studentId, _) = await SeedQuizAttemptAsync();

        var response = await client.GetAsync($"/students/{studentId}/quiz-attempts");

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task GetStudentQuizAttempts_WhenStudent_ReturnsQuizAttemptDtos()
    {
        using var client = factory.CreateAppHttpClient();
        await client.AuthenticateAsync(TestUsers.Student.User.Email!, TestUsers.Student.Password, "Student");
        var (_, studentId, _) = await SeedQuizAttemptAsync();

        var response = await client.GetAsync($"/students/{studentId}/quiz-attempts");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var attempts = await response.Content.ReadFromJsonAsync<IReadOnlyList<QuizAttemptDto>>();
        attempts.Should().NotBeNull();
    }

    [Fact]
    public async Task GetStudentQuizAttemptsCount_WhenInstructor_ReturnsForbidden()
    {
        using var client = factory.CreateAppHttpClient();
        await client.AuthenticateAsync(TestUsers.Instructor1.User.Email!, TestUsers.Instructor1.Password, "Instructor");
        var (_, studentId, _) = await GetSeededIdsAsync();

        var response = await client.GetAsync($"/students/{studentId}/quiz-attempts/count");

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task GetStudentQuizAttemptsCount_WhenStudent_ReturnsQuizAttemptsCountDto()
    {
        using var client = factory.CreateAppHttpClient();
        await client.AuthenticateAsync(TestUsers.Student.User.Email!, TestUsers.Student.Password, "Student");
        var (_, studentId, _) = await SeedQuizAttemptAsync();

        var response = await client.GetAsync($"/students/{studentId}/quiz-attempts/count");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var countDto = await response.Content.ReadFromJsonAsync<QuizAttemptsCountDto>();
        countDto.Should().NotBeNull();
        countDto.QuizAttemptCount.Should().BeGreaterThanOrEqualTo(0);
    }

    [Fact]
    public async Task GetStudentQuizAttemptsCount_WhenAdmin_ReturnsQuizAttemptsCountDto()
    {
        using var client = factory.CreateAppHttpClient();
        await client.AuthenticateAsync(TestUsers.Admin.User.Email!, TestUsers.Admin.Password, "Admin");
        var (_, studentId, _) = await SeedQuizAttemptAsync();

        var response = await client.GetAsync($"/students/{studentId}/quiz-attempts/count");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var countDto = await response.Content.ReadFromJsonAsync<QuizAttemptsCountDto>();
        countDto.Should().NotBeNull();
    }

    [Fact]
    public async Task GetAllQuizzesAttempts_WhenStudent_ReturnsForbidden()
    {
        using var client = factory.CreateAppHttpClient();
        await client.AuthenticateAsync(TestUsers.Student.User.Email!, TestUsers.Student.Password, "Student");

        var response = await client.GetAsync("/quiz-attempts");

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task GetAllQuizzesAttempts_WhenInstructor_ReturnsForbidden()
    {
        using var client = factory.CreateAppHttpClient();
        await client.AuthenticateAsync(TestUsers.Instructor1.User.Email!, TestUsers.Instructor1.Password, "Instructor");

        var response = await client.GetAsync("/quiz-attempts");

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task GetAllQuizzesAttempts_WhenAdmin_ReturnsPaginatedQuizAttempts()
    {
        using var client = factory.CreateAppHttpClient();
        await client.AuthenticateAsync(TestUsers.Admin.User.Email!, TestUsers.Admin.Password, "Admin");
        await SeedQuizAttemptAsync();

        var response = await client.GetAsync("/quiz-attempts");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var attempts = await response.Content.ReadFromJsonAsync<PaginatedList<QuizAttemptDto>>();
        attempts.Should().NotBeNull();
        attempts.Items.Should().NotBeNull();
    }

    [Theory]
    [InlineData(0, 10)]
    [InlineData(-1, 10)]
    [InlineData(1, 0)]
    [InlineData(1, -5)]
    [InlineData(1, 101)]
    public async Task GetAllQuizzesAttempts_WithInvalidPagination_ReturnsBadRequest(int pageNumber, int pageSize)
    {
        using var client = factory.CreateAppHttpClient();
        await client.AuthenticateAsync(TestUsers.Admin.User.Email!, TestUsers.Admin.Password, "Admin");

        var response = await client.GetAsync($"/quiz-attempts?PageNumber={pageNumber}&PageSize={pageSize}");

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    private async Task<(Guid attemptId, Guid studentId, Guid quizId)> SeedQuizAttemptAsync()
    {
        using var scope = factory.Services.CreateScope();
        var mongoContext = scope.ServiceProvider.GetRequiredService<IMongoDbContext>();
        var (quiz, questions, studentId) = await CreateQuizAsync(mongoContext, active: false);
        var attemptId = Guid.NewGuid();
        var answers = questions
            .Select(question => ((Tf)question).Solve(true, studentId, attemptId).Value)
            .ToList();
        var attempt = QuizAttempt.Start(
            attemptId,
            studentId,
            quiz.Id,
            DateTime.UtcNow.AddMinutes(-10),
            quiz.EndsAtUtc).Value;
        foreach (var answer in answers)
        {
            attempt.SubmitAnswer(answer);
        }

        await mongoContext.Quizzes.InsertOneAsync(quiz);
        await mongoContext.QuizAttempts.InsertOneAsync(attempt);

        return (attemptId, studentId, quiz.Id);
    }

    private async Task<(Quiz quiz, List<Question> questions, Guid studentId)> CreateQuizAsync(
        IMongoDbContext mongoContext,
        bool active)
    {
        var course = await mongoContext.Courses.Find(_ => true).FirstAsync();
        var instructor = await mongoContext.Users.Find(u => u.UserRole == UserRole.Instructor && u.Id == course.InstructorId).FirstAsync();
        var student = await mongoContext.Users.Find(u => u.UserRole == UserRole.Student && u.PersonalInformation.Email == TestUsers.Student.User.Email).FirstAsync();
        var quizId = Guid.NewGuid();
        var questionArgs = Enumerable.Range(0, 3)
            .Select(index => new CreateTfArgs(
                $"Attempt question {index + 1}",
                10,
                true))
            .Cast<CreateQuestionArgs>()
            .ToList();
        var startsAt = active ? DateTimeOffset.UtcNow.AddMinutes(-10) : DateTimeOffset.UtcNow.AddDays(-2);
        var endsAt = active ? DateTimeOffset.UtcNow.AddMinutes(10) : DateTimeOffset.UtcNow.AddDays(-1);
        var quiz = Quiz.Create(
            quizId,
            course.Id,
            instructor.Id,
            course.Name,
            instructor.PersonalInformation.Name,
            $"Attempt {Guid.NewGuid():N}"[..20],
            startsAt,
            endsAt,
            questionArgs,
            course).Value;

        return (quiz, [.. quiz.Questions], student.Id);
    }

    private async Task<(Guid courseId, Guid studentId, Guid instructorId)> GetSeededIdsAsync()
    {
        using var scope = factory.Services.CreateScope();
        var mongoContext = scope.ServiceProvider.GetRequiredService<IMongoDbContext>();
        var course = await mongoContext.Courses.Find(_ => true).FirstAsync();
        var student = await mongoContext.Users.Find(u => u.UserRole == UserRole.Student).FirstAsync();
        var instructor = await mongoContext.Users.Find(u => u.UserRole == UserRole.Instructor).FirstAsync();

        return (course.Id, student.Id, instructor.Id);
    }
}
