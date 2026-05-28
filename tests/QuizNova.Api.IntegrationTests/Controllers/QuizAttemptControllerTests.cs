using System.Net;

using FluentAssertions;

using Microsoft.EntityFrameworkCore;

using QuizNova.Api.DTOs.Requests;
using QuizNova.Api.IntegrationTests.Common;
using QuizNova.Application.Common.Interfaces;
using QuizNova.Application.Common.Models;
using QuizNova.Application.Features.QuizAttempts.DTOs;
using QuizNova.Domain.Entities.Enrollments;
using QuizNova.Domain.Entities.QuizAttempts;
using QuizNova.Domain.Entities.Quizzes;
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
        await client.AuthenticateAsync(TestUsers.Student.User.Email!, TestUsers.Student.Password);
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
        await client.AuthenticateAsync(TestUsers.Admin.User.Email!, TestUsers.Admin.Password);
        var (attemptId, studentId, _) = await SeedQuizAttemptAsync();

        var response = await client.GetAsync($"/students/{studentId}/quiz-attempts/{attemptId}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetQuizAttemptById_WithNonExistentId_ReturnsNotFound()
    {
        using var client = factory.CreateAppHttpClient();
        await client.AuthenticateAsync(TestUsers.Student.User.Email!, TestUsers.Student.Password);
        var (_, studentId, _) = await GetSeededIdsAsync();

        var response = await client.GetAsync($"/students/{studentId}/quiz-attempts/{Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetQuizAttemptById_WithEmptyId_ReturnsBadRequest()
    {
        using var client = factory.CreateAppHttpClient();
        await client.AuthenticateAsync(TestUsers.Student.User.Email!, TestUsers.Student.Password);
        var (_, studentId, _) = await GetSeededIdsAsync();

        var response = await client.GetAsync($"/students/{studentId}/quiz-attempts/{Guid.Empty}");

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetQuizAttemptByIdForGrading_WhenStudent_ReturnsForbidden()
    {
        using var client = factory.CreateAppHttpClient();
        await client.AuthenticateAsync(TestUsers.Student.User.Email!, TestUsers.Student.Password);
        var (attemptId, _, _) = await SeedQuizAttemptAsync();

        var response = await client.GetAsync($"/quiz-attempts/{attemptId}");

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task GetQuizAttemptByIdForGrading_WhenAdmin_ReturnsForbidden()
    {
        using var client = factory.CreateAppHttpClient();
        await client.AuthenticateAsync(TestUsers.Admin.User.Email!, TestUsers.Admin.Password);
        var (attemptId, _, _) = await SeedQuizAttemptAsync();

        var response = await client.GetAsync($"/quiz-attempts/{attemptId}");

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task GetQuizAttemptByIdForGrading_WhenInstructor_ReturnsQuizAttemptDto()
    {
        using var client = factory.CreateAppHttpClient();
        await client.AuthenticateAsync(TestUsers.Instructor.User.Email!, TestUsers.Instructor.Password);
        var (attemptId, _, _) = await SeedQuizAttemptAsync();

        var response = await client.GetAsync($"/quiz-attempts/{attemptId}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var attempt = await response.Content.ReadFromJsonAsync<QuizAttemptDto>();
        attempt.Should().NotBeNull();
        attempt.QuizAttemptId.Should().Be(attemptId);
    }

    [Fact]
    public async Task SubmitQuizAttempt_WhenInstructor_ReturnsForbidden()
    {
        using var client = factory.CreateAppHttpClient();
        await client.AuthenticateAsync(TestUsers.Instructor.User.Email!, TestUsers.Instructor.Password);
        var (quizId, studentId, questions) = await SeedActiveQuizForSubmissionAsync();
        var request = CreateSubmitRequest(quizId, questions);

        var response = await client.PostAsJsonAsync($"/students/{studentId}/quiz-attempts", request);

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task SubmitQuizAttempt_WhenAdmin_ReturnsForbidden()
    {
        using var client = factory.CreateAppHttpClient();
        await client.AuthenticateAsync(TestUsers.Admin.User.Email!, TestUsers.Admin.Password);
        var (quizId, studentId, questions) = await SeedActiveQuizForSubmissionAsync();
        var request = CreateSubmitRequest(quizId, questions);

        var response = await client.PostAsJsonAsync($"/students/{studentId}/quiz-attempts", request);

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task SubmitQuizAttempt_WhenStudent_ReturnsQuizAttemptDto()
    {
        using var client = factory.CreateAppHttpClient();
        await client.AuthenticateAsync(TestUsers.Student.User.Email!, TestUsers.Student.Password);
        var (quizId, studentId, questions) = await SeedActiveQuizForSubmissionAsync();
        var request = CreateSubmitRequest(quizId, questions);

        var response = await client.PostAsJsonAsync($"/students/{studentId}/quiz-attempts", request);

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var attempt = await response.Content.ReadFromJsonAsync<QuizAttemptDto>();
        attempt.Should().NotBeNull();
        attempt.QuizId.Should().Be(quizId);
    }

    [Fact]
    public async Task GetStudentQuizAttempts_WhenAdmin_ReturnsForbidden()
    {
        using var client = factory.CreateAppHttpClient();
        await client.AuthenticateAsync(TestUsers.Admin.User.Email!, TestUsers.Admin.Password);
        var (_, studentId, _) = await SeedQuizAttemptAsync();

        var response = await client.GetAsync($"/students/{studentId}/quiz-attempts");

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task GetStudentQuizAttempts_WhenStudent_ReturnsQuizAttemptDtos()
    {
        using var client = factory.CreateAppHttpClient();
        await client.AuthenticateAsync(TestUsers.Student.User.Email!, TestUsers.Student.Password);
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
        await client.AuthenticateAsync(TestUsers.Instructor.User.Email!, TestUsers.Instructor.Password);
        var (_, studentId, _) = await GetSeededIdsAsync();

        var response = await client.GetAsync($"/students/{studentId}/quiz-attempts/count");

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task GetStudentQuizAttemptsCount_WhenStudent_ReturnsQuizAttemptsCountDto()
    {
        using var client = factory.CreateAppHttpClient();
        await client.AuthenticateAsync(TestUsers.Student.User.Email!, TestUsers.Student.Password);
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
        await client.AuthenticateAsync(TestUsers.Admin.User.Email!, TestUsers.Admin.Password);
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
        await client.AuthenticateAsync(TestUsers.Student.User.Email!, TestUsers.Student.Password);

        var response = await client.GetAsync("/quiz-attempts");

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task GetAllQuizzesAttempts_WhenInstructor_ReturnsForbidden()
    {
        using var client = factory.CreateAppHttpClient();
        await client.AuthenticateAsync(TestUsers.Instructor.User.Email!, TestUsers.Instructor.Password);

        var response = await client.GetAsync("/quiz-attempts");

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task GetAllQuizzesAttempts_WhenAdmin_ReturnsPaginatedQuizAttempts()
    {
        using var client = factory.CreateAppHttpClient();
        await client.AuthenticateAsync(TestUsers.Admin.User.Email!, TestUsers.Admin.Password);
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
        await client.AuthenticateAsync(TestUsers.Admin.User.Email!, TestUsers.Admin.Password);

        var response = await client.GetAsync($"/quiz-attempts?PageNumber={pageNumber}&PageSize={pageSize}");

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    private static SubmitQuizAttemptRequest CreateSubmitRequest(Guid quizId, IReadOnlyList<Guid> questionIds)
    {
        return new SubmitQuizAttemptRequest(
            quizId,
            DateTimeOffset.UtcNow.AddMinutes(-5),
            DateTimeOffset.UtcNow.AddMinutes(-1),
            questionIds.Select(questionId => new SubmitTfAnswerRequest(questionId, true))
                .Cast<SubmitQuestionAnswerRequest>()
                .ToList());
    }

    private static List<Tf> CreateQuestions(Guid quizId)
    {
        return Enumerable.Range(0, 3)
            .Select(index => Tf.Create(
                Guid.NewGuid(),
                quizId,
                $"Attempt question {index + 1}",
                true,
                index,
                10).Value)
            .ToList();
    }

    private async Task<(Guid attemptId, Guid studentId, Guid quizId)> SeedQuizAttemptAsync()
    {
        using var scope = factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<IAppDbContext>();
        var (quiz, questions, studentId) = await CreateQuizAsync(dbContext, active: false);
        var attemptId = Guid.NewGuid();
        var answers = questions
            .Select(question => question.Solve(true, studentId, attemptId).Value)
            .ToList();
        var attempt = QuizAttempt.Create(
            attemptId,
            studentId,
            quiz.Id,
            DateTime.UtcNow.AddMinutes(-10),
            DateTime.UtcNow.AddMinutes(-5),
            answers).Value;

        await dbContext.Quizzes.AddAsync(quiz);
        await dbContext.Questions.AddRangeAsync(questions);
        await dbContext.QuizAttempts.AddAsync(attempt);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        return (attemptId, studentId, quiz.Id);
    }

    private async Task<(Guid quizId, Guid studentId, IReadOnlyList<Guid> questionIds)>
        SeedActiveQuizForSubmissionAsync()
    {
        using var scope = factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<IAppDbContext>();
        var (quiz, questions, studentId) = await CreateQuizAsync(dbContext, active: true);
        var existingEnrollment = await dbContext.Enrollments
            .AnyAsync(enrollment => enrollment.StudentId == studentId && enrollment.CourseId == quiz.CourseId);

        if (!existingEnrollment)
        {
            var enrollment = Enrollment.Create(Guid.NewGuid(), studentId, quiz.CourseId, DateTimeOffset.UtcNow).Value;
            await dbContext.Enrollments.AddAsync(enrollment);
        }

        await dbContext.Quizzes.AddAsync(quiz);
        await dbContext.Questions.AddRangeAsync(questions);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        return (quiz.Id, studentId, questions.Select(question => question.Id).ToList());
    }

    private async Task<(Quiz quiz, List<Tf> questions, Guid studentId)> CreateQuizAsync(
        IAppDbContext dbContext,
        bool active)
    {
        var course = await dbContext.Courses.FirstAsync();
        var instructor = await dbContext.Instructors.FirstAsync(instructor => instructor.Id == course.InstructorId);
        var student = await dbContext.Students
            .FirstAsync(s => s.PersonalInformation.Email == TestUsers.Student.User.Email);
        var quizId = Guid.NewGuid();
        var questions = CreateQuestions(quizId);
        var startsAt = active ? DateTimeOffset.UtcNow.AddMinutes(-10) : DateTimeOffset.UtcNow.AddDays(-2);
        var endsAt = active ? DateTimeOffset.UtcNow.AddMinutes(10) : DateTimeOffset.UtcNow.AddDays(-1);
        var quiz = Quiz.Create(
            quizId,
            course.Id,
            instructor.Id,
            $"Attempt {Guid.NewGuid():N}"[..20],
            startsAt,
            endsAt,
            questions.Cast<Question>().ToList()).Value;

        return (quiz, questions, student.Id);
    }

    private async Task<(Guid courseId, Guid studentId, Guid instructorId)> GetSeededIdsAsync()
    {
        using var scope = factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<IAppDbContext>();
        var course = await dbContext.Courses.FirstAsync();
        var student = await dbContext.Students.FirstAsync();
        var instructor = await dbContext.Instructors.FirstAsync();

        return (course.Id, student.Id, instructor.Id);
    }
}
