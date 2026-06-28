using System.Net;

using FluentAssertions;

using Microsoft.EntityFrameworkCore;

using QuizNova.Api.DTOs.Requests;
using QuizNova.Api.IntegrationTests.Common;
using QuizNova.Application.Common.Interfaces;
using QuizNova.Application.Common.Models;
using QuizNova.Application.Features.Quizzes.DTOs;
using QuizNova.Domain.Entities.Quizzes;
using QuizNova.Domain.Entities.Quizzes.Questions.AutoGradedQuestions.TrueFalse;
using QuizNova.Domain.Entities.Quizzes.Questions.Base;
using QuizNova.Tests.Common.Security;

using Xunit;

namespace QuizNova.Api.IntegrationTests.Controllers;

public class QuizControllerTests(CustomWebApplicationFactory factory) : IClassFixture<CustomWebApplicationFactory>
{
    [Fact]
    public async Task GetAllQuizzes_WhenUnauthenticated_ReturnsUnauthorized()
    {
        using var client = factory.CreateAppHttpClient();

        var response = await client.GetAsync("/quizzes");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetAllQuizzes_WhenStudent_ReturnsForbidden()
    {
        using var client = factory.CreateAppHttpClient();
        await client.AuthenticateAsync(TestUsers.Student.User.Email!, TestUsers.Student.Password, "Student");

        var response = await client.GetAsync("/quizzes");

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task GetAllQuizzes_WhenInstructor_ReturnsForbidden()
    {
        using var client = factory.CreateAppHttpClient();
        await client.AuthenticateAsync(TestUsers.Instructor1.User.Email!, TestUsers.Instructor1.Password, "Instructor");

        var response = await client.GetAsync("/quizzes");

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task GetAllQuizzes_WhenAdmin_ReturnsOkAndPaginatedQuizzes()
    {
        using var client = factory.CreateAppHttpClient();
        await client.AuthenticateAsync(TestUsers.Admin.User.Email!, TestUsers.Admin.Password, "Admin");
        await SeedQuizAsync();

        var response = await client.GetAsync("/quizzes");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<PaginatedList<QuizDto>>();
        result.Should().NotBeNull();
        result.Items.Should().NotBeNull();
    }

    [Theory]
    [InlineData(0, 10)]
    [InlineData(-1, 10)]
    [InlineData(1, 0)]
    [InlineData(1, -5)]
    [InlineData(1, 101)]
    public async Task GetAllQuizzes_WithInvalidPagination_ReturnsBadRequest(int pageNumber, int pageSize)
    {
        using var client = factory.CreateAppHttpClient();
        await client.AuthenticateAsync(TestUsers.Admin.User.Email!, TestUsers.Admin.Password, "Admin");

        var response = await client.GetAsync($"/quizzes?PageNumber={pageNumber}&PageSize={pageSize}");

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetInstructorQuizzesCount_WhenStudent_ReturnsForbidden()
    {
        using var client = factory.CreateAppHttpClient();
        await client.AuthenticateAsync(TestUsers.Student.User.Email!, TestUsers.Student.Password, "Student");
        var (_, instructorId, _) = await GetSeededIdsAsync();

        var response = await client.GetAsync($"/quizzes/count?instructorId={instructorId}");

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task GetInstructorQuizzesCount_WhenInstructor_ReturnsQuizzesCountDto()
    {
        using var client = factory.CreateAppHttpClient();
        await client.AuthenticateAsync(TestUsers.Instructor1.User.Email!, TestUsers.Instructor1.Password, "Instructor");
        var (_, instructorId, _) = await GetSeededIdsAsync();

        var response = await client.GetAsync($"/quizzes/count?instructorId={instructorId}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var countDto = await response.Content.ReadFromJsonAsync<QuizzesCountDto>();
        countDto.Should().NotBeNull();
        countDto.QuizzesCount.Should().BeGreaterThanOrEqualTo(0);
    }

    [Fact]
    public async Task GetInstructorQuizzesCount_WhenAdmin_ReturnsQuizzesCountDto()
    {
        using var client = factory.CreateAppHttpClient();
        await client.AuthenticateAsync(TestUsers.Admin.User.Email!, TestUsers.Admin.Password, "Admin");
        var (_, instructorId, _) = await GetSeededIdsAsync();

        var response = await client.GetAsync($"/quizzes/count?instructorId={instructorId}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var countDto = await response.Content.ReadFromJsonAsync<QuizzesCountDto>();
        countDto.Should().NotBeNull();
        countDto.QuizzesCount.Should().BeGreaterThanOrEqualTo(0);
    }

    [Fact]
    public async Task GetInstructorQuizzesCount_WithEmptyInstructorId_ReturnsBadRequest()
    {
        using var client = factory.CreateAppHttpClient();
        await client.AuthenticateAsync(TestUsers.Admin.User.Email!, TestUsers.Admin.Password, "Admin");

        var response = await client.GetAsync($"/quizzes/count?instructorId={Guid.Empty}");

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetQuizById_WithValidId_ReturnsQuizDto()
    {
        using var client = factory.CreateAppHttpClient();
        await client.AuthenticateAsync(TestUsers.Student.User.Email!, TestUsers.Student.Password, "Student");
        var quizId = await SeedQuizAsync();

        var response = await client.GetAsync($"/quizzes/{quizId}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var quiz = await response.Content.ReadFromJsonAsync<QuizDto>();
        quiz.Should().NotBeNull();
        quiz.QuizId.Should().Be(quizId);
    }

    [Fact]
    public async Task GetQuizById_WithNonExistentId_ReturnsNotFound()
    {
        using var client = factory.CreateAppHttpClient();
        await client.AuthenticateAsync(TestUsers.Student.User.Email!, TestUsers.Student.Password, "Student");

        var response = await client.GetAsync($"/quizzes/{Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetQuizById_WithEmptyId_ReturnsBadRequest()
    {
        using var client = factory.CreateAppHttpClient();
        await client.AuthenticateAsync(TestUsers.Student.User.Email!, TestUsers.Student.Password, "Student");

        var response = await client.GetAsync($"/quizzes/{Guid.Empty}");

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateQuiz_WhenStudent_ReturnsForbidden()
    {
        using var client = factory.CreateAppHttpClient();
        await client.AuthenticateAsync(TestUsers.Student.User.Email!, TestUsers.Student.Password, "Student");
        var request = await CreateValidQuizRequestAsync();

        var response = await client.PostAsJsonAsync("/quizzes", request);

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task CreateQuiz_WhenAdmin_ReturnsForbidden()
    {
        using var client = factory.CreateAppHttpClient();
        await client.AuthenticateAsync(TestUsers.Admin.User.Email!, TestUsers.Admin.Password, "Admin");
        var request = await CreateValidQuizRequestAsync();

        var response = await client.PostAsJsonAsync("/quizzes", request);

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task CreateQuiz_WhenInstructor_ReturnsQuizDto()
    {
        using var client = factory.CreateAppHttpClient();
        await client.AuthenticateAsync(TestUsers.Instructor1.User.Email!, TestUsers.Instructor1.Password, "Instructor");
        var request = await CreateValidQuizRequestAsync();

        var response = await client.PostAsJsonAsync("/quizzes", request);

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var quiz = await response.Content.ReadFromJsonAsync<QuizDto>();
        quiz.Should().NotBeNull();
        quiz.Title.Should().Be(request.Title);
    }

    [Fact]
    public async Task UpdateQuizMetadata_WhenStudent_ReturnsForbidden()
    {
        using var client = factory.CreateAppHttpClient();
        await client.AuthenticateAsync(TestUsers.Student.User.Email!, TestUsers.Student.Password, "Student");
        var quizId = await SeedQuizAsync();
        var request = new UpdateQuizMetadataRequest(
            "Updated Quiz",
            DateTimeOffset.UtcNow.AddDays(3),
            DateTimeOffset.UtcNow.AddDays(4));

        var response = await client.PutAsJsonAsync($"/quizzes/{quizId}/metadata", request);

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task UpdateQuizMetadata_WhenInstructor_ReturnsNoContent()
    {
        using var client = factory.CreateAppHttpClient();
        await client.AuthenticateAsync(TestUsers.Instructor1.User.Email!, TestUsers.Instructor1.Password, "Instructor");
        var quizId = await SeedQuizAsync();
        var request = new UpdateQuizMetadataRequest(
            "Updated Quiz",
            DateTimeOffset.UtcNow.AddDays(3),
            DateTimeOffset.UtcNow.AddDays(4));

        var response = await client.PutAsJsonAsync($"/quizzes/{quizId}/metadata", request);

        response.StatusCode.Should().Be(HttpStatusCode.NoContent, await response.Content.ReadAsStringAsync());
    }

    [Fact]
    public async Task AddQuestion_WhenInstructor_ReturnsCreatedQuestion()
    {
        using var client = factory.CreateAppHttpClient();
        await client.AuthenticateAsync(TestUsers.Instructor1.User.Email!, TestUsers.Instructor1.Password, "Instructor");
        var quizId = await SeedQuizAsync();
        var request = new CreateTfRequest("Additional true false question", 5, true);

        var response = await PostQuestionAsync(client, quizId, request);

        response.StatusCode.Should().Be(HttpStatusCode.OK, await response.Content.ReadAsStringAsync());

        var question = await response.Content.ReadFromJsonAsync<TfDto>();
        question.Should().NotBeNull();
    }

    [Fact]
    public async Task UpdateQuestion_WhenInstructor_ReturnsNoContent()
    {
        using var client = factory.CreateAppHttpClient();
        await client.AuthenticateAsync(TestUsers.Instructor1.User.Email!, TestUsers.Instructor1.Password, "Instructor");
        var (quizId, questionId) = await SeedQuizWithQuestionAsync();
        var request = new UpdateTfRequest("Updated true false question", 0, 10, false);

        var response = await PutQuestionAsync(client, quizId, questionId, request);

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task UpdateQuizCourseId_WhenInstructor_ReturnsNoContent()
    {
        using var client = factory.CreateAppHttpClient();
        await client.AuthenticateAsync(TestUsers.Instructor1.User.Email!, TestUsers.Instructor1.Password, "Instructor");
        var quizId = await SeedQuizAsync();
        var (courseId, _, _) = await GetAlternateSeededIdsAsync();
        var request = new UpdateQuizCourseIdRequest(courseId);

        var response = await client.PutAsJsonAsync($"/quizzes/{quizId}/course", request);

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task DeleteQuestion_WhenInstructor_ReturnsNoContent()
    {
        using var client = factory.CreateAppHttpClient();
        await client.AuthenticateAsync(TestUsers.Instructor1.User.Email!, TestUsers.Instructor1.Password, "Instructor");
        var (quizId, questionId) = await SeedQuizWithQuestionAsync(questionCount: 6);

        var response = await client.DeleteAsync($"/quizzes/{quizId}/questions/{questionId}");

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task GetStudentQuizzes_WhenAdmin_ReturnsForbidden()
    {
        using var client = factory.CreateAppHttpClient();
        await client.AuthenticateAsync(TestUsers.Admin.User.Email!, TestUsers.Admin.Password, "Admin");
        var (_, _, studentId) = await GetSeededIdsAsync();

        var response = await client.GetAsync($"/students/{studentId}/quizzes");

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task GetStudentQuizzes_WhenStudent_ReturnsStudentQuizzesDto()
    {
        using var client = factory.CreateAppHttpClient();
        await client.AuthenticateAsync(TestUsers.Student.User.Email!, TestUsers.Student.Password, "Student");
        var (_, _, studentId) = await GetSeededIdsAsync();

        var response = await client.GetAsync($"/students/{studentId}/quizzes");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var quizzes = await response.Content.ReadFromJsonAsync<StudentQuizzesDto>();
        quizzes.Should().NotBeNull();
    }

    private static async Task<HttpResponseMessage> PostQuestionAsync(
        AppHttpClient client,
        Guid quizId,
        CreateQuizQuestionRequest request)
    {
        var message = new HttpRequestMessage(HttpMethod.Post, $"/quizzes/{quizId}/questions");
        message.Content = JsonContent.Create(request);

        return await client.SendAsync(message);
    }

    private static async Task<HttpResponseMessage> PutQuestionAsync(
        AppHttpClient client,
        Guid quizId,
        Guid questionId,
        UpdateQuestionRequest request)
    {
        using var message = new HttpRequestMessage(HttpMethod.Put, $"/quizzes/{quizId}/questions/{questionId}");
        message.Content = JsonContent.Create(request);

        return await client.SendAsync(message);
    }

    private async Task<CreateQuizRequest> CreateValidQuizRequestAsync()
    {
        var (courseId, instructorId, _) = await GetSeededIdsAsync();
        return new CreateQuizRequest(
            "TESTING QUIZ TITLE",
            courseId,
            instructorId,
            DateTimeOffset.UtcNow.AddDays(1),
            DateTimeOffset.UtcNow.AddDays(2),
            [
                new CreateTfRequest("Question one text", 5, true),
                new CreateTfRequest("Question two text", 5, false),
                new CreateTfRequest("Question three text", 5, true),
            ]);
    }

    private async Task<Guid> SeedQuizAsync()
    {
        var (quizId, _) = await SeedQuizWithQuestionAsync();
        return quizId;
    }

    private async Task<(Guid quizId, Guid questionId)> SeedQuizWithQuestionAsync(int questionCount = 3)
    {
        using var scope = factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<IAppDbContext>();
        var course = await dbContext.Courses.FirstAsync();
        var instructor = await dbContext.Instructors.FirstAsync(instructor => instructor.Id == course.InstructorId);
        var quizId = Guid.NewGuid();
        var questions = Enumerable.Range(0, questionCount)
            .Select(index => Tf.Create(
                Guid.NewGuid(),
                quizId,
                $"Seed question {index + 1}",
                index % 2 == 0,
                index,
                10).Value)
            .Cast<Question>()
            .ToList();
        var quiz = Quiz.Create(
            quizId,
            course.Id,
            instructor.Id,
            $"Seed {Guid.NewGuid():N}"[..20],
            DateTimeOffset.UtcNow.AddDays(1),
            DateTimeOffset.UtcNow.AddDays(2),
            questions).Value;

        await dbContext.Quizzes.AddAsync(quiz);
        await dbContext.Questions.AddRangeAsync(questions);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        return (quizId, questions[0].Id);
    }

    private async Task<(Guid courseId, Guid instructorId, Guid studentId)> GetSeededIdsAsync()
    {
        using var scope = factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<IAppDbContext>();
        var course = await dbContext.Courses.FirstAsync();
        var instructor = await dbContext.Instructors.FirstAsync(instructor => instructor.Id == course.InstructorId);
        var student = await dbContext.Students.FirstAsync();

        return (course.Id, instructor.Id, student.Id);
    }

    private async Task<(Guid courseId, Guid instructorId, Guid studentId)> GetAlternateSeededIdsAsync()
    {
        using var scope = factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<IAppDbContext>();
        var course = await dbContext.Courses.OrderByDescending(c => c.Name).FirstAsync();
        var instructor = await dbContext.Instructors.FirstAsync(instructor => instructor.Id == course.InstructorId);
        var student = await dbContext.Students.FirstAsync();

        return (course.Id, instructor.Id, student.Id);
    }
}
