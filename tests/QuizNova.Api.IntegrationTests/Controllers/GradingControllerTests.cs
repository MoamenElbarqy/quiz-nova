using System.Net;

using FluentAssertions;

using Microsoft.EntityFrameworkCore;

using QuizNova.Api.DTOs.Requests;
using QuizNova.Api.IntegrationTests.Common;
using QuizNova.Application.Common.Interfaces;
using QuizNova.Application.Common.Models;
using QuizNova.Application.Features.QuizAttempts.DTOs;
using QuizNova.Domain.Entities.QuizAttempts;
using QuizNova.Domain.Entities.QuizAttempts.Answers.ManuallyGradedAnswers.EssayAnswer;
using QuizNova.Domain.Entities.Quizzes;
using QuizNova.Domain.Entities.Quizzes.Questions.Base;
using QuizNova.Domain.Entities.Quizzes.Questions.ManuallyGradedQuestions;
using QuizNova.Tests.Common.Security;

using Xunit;

namespace QuizNova.Api.IntegrationTests.Controllers;

public class GradingControllerTests(CustomWebApplicationFactory factory) : IClassFixture<CustomWebApplicationFactory>
{
    [Fact]
    public async Task GetPendingManualAnswers_WhenUnauthenticated_ReturnsUnauthorized()
    {
        // Arrange
        using var client = factory.CreateAppHttpClient();

        // Act
        var response = await client.GetAsync("/quiz-attempts/manually-graded-answers");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetPendingManualAnswers_WhenStudent_ReturnsForbidden()
    {
        // Arrange
        using var client = factory.CreateAppHttpClient();
        await client.AuthenticateAsync(TestUsers.Student.User.Email!, TestUsers.Student.Password, "Student");

        // Act
        var response = await client.GetAsync("/quiz-attempts/manually-graded-answers");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task GetPendingManualAnswers_WhenAdmin_ReturnsForbidden()
    {
        // Arrange
        using var client = factory.CreateAppHttpClient();
        await client.AuthenticateAsync(TestUsers.Admin.User.Email!, TestUsers.Admin.Password, "Admin");

        // Act
        var response = await client.GetAsync("/quiz-attempts/manually-graded-answers");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task GetPendingManualAnswers_WhenValidInstructor_ReturnsOkAndPaginatedList()
    {
        // Arrange
        using var client = factory.CreateAppHttpClient();
        await client.AuthenticateAsync(TestUsers.Instructor1.User.Email!, TestUsers.Instructor1.Password, "Instructor");

        // Seed an answer belonging to this instructor
        await SeedManualAnswerAsync(TestUsers.Instructor1.User.Email!);

        // Act
        var response = await client.GetAsync("/quiz-attempts/manually-graded-answers");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<PaginatedList<PendingManualAnswersDto>>();
        result.Should().NotBeNull();
        result!.Items.Should().NotBeEmpty();
    }

    [Theory]
    [InlineData(0, 10)]
    [InlineData(-1, 10)]
    [InlineData(1, 0)]
    [InlineData(1, -5)]
    [InlineData(1, 101)]
    public async Task GetPendingManualAnswers_WithInvalidPagination_ReturnsBadRequest(int pageNumber, int pageSize)
    {
        // Arrange
        using var client = factory.CreateAppHttpClient();
        await client.AuthenticateAsync(TestUsers.Instructor1.User.Email!, TestUsers.Instructor1.Password, "Instructor");

        // Act
        var response =
            await client.GetAsync(
                $"/quiz-attempts/manually-graded-answers?PageNumber={pageNumber}&PageSize={pageSize}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GradeQuestion_WhenValid_ReturnsNoContent()
    {
        // Arrange
        using var client = factory.CreateAppHttpClient();
        await client.AuthenticateAsync(TestUsers.Instructor1.User.Email!, TestUsers.Instructor1.Password, "Instructor");

        var (_, answerId, _) = await SeedManualAnswerAsync(TestUsers.Instructor1.User.Email!);
        var request = new GradeQuestionRequest(8, "Excellent explanation of REST API principles!");

        // Act
        var response = await client.PutAsJsonAsync($"/quiz-attempts/manually-graded-answers/{answerId}", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task GradeQuestion_WithInvalidScore_ReturnsBadRequest()
    {
        // Arrange
        using var client = factory.CreateAppHttpClient();
        await client.AuthenticateAsync(TestUsers.Instructor1.User.Email!, TestUsers.Instructor1.Password, "Instructor");

        var (_, answerId, _) = await SeedManualAnswerAsync(TestUsers.Instructor1.User.Email!);

        // Negative score is invalid
        var request = new GradeQuestionRequest(-5, "Score is invalid.");

        // Act
        var response = await client.PutAsJsonAsync($"/quiz-attempts/manually-graded-answers/{answerId}", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GradeQuestion_WithNonExistentAnswer_ReturnsNotFound()
    {
        // Arrange
        using var client = factory.CreateAppHttpClient();
        await client.AuthenticateAsync(TestUsers.Instructor1.User.Email!, TestUsers.Instructor1.Password, "Instructor");

        var nonExistentId = Guid.NewGuid();
        var request = new GradeQuestionRequest(5, "Valid feedback");

        // Act
        var response = await client.PutAsJsonAsync($"/quiz-attempts/manually-graded-answers/{nonExistentId}", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GradeQuestion_Twice_ReturnsConflict()
    {
        // Arrange
        using var client = factory.CreateAppHttpClient();
        await client.AuthenticateAsync(TestUsers.Instructor1.User.Email!, TestUsers.Instructor1.Password, "Instructor");

        var (_, answerId, _) = await SeedManualAnswerAsync(TestUsers.Instructor1.User.Email!);
        var firstRequest = new GradeQuestionRequest(7, "First valid grade");
        var secondRequest = new GradeQuestionRequest(9, "Trying to grade a second time");

        // Act
        // 1. Grade the first time
        var firstResponse =
            await client.PutAsJsonAsync($"/quiz-attempts/manually-graded-answers/{answerId}", firstRequest);
        firstResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // 2. Grade a second time (should be refused!)
        var secondResponse =
            await client.PutAsJsonAsync($"/quiz-attempts/manually-graded-answers/{answerId}", secondRequest);

        // Assert
        secondResponse.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    private async Task<(Guid attemptId, Guid answerId, Guid questionId)> SeedManualAnswerAsync(string instructorEmail)
    {
        using var scope = factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<IAppDbContext>();

        var instructor = await dbContext.Instructors
                             .Include(i => i.Courses)
                             .FirstOrDefaultAsync(i => i.PersonalInformation.Email == instructorEmail)
                         ?? throw new InvalidOperationException($"Instructor {instructorEmail} not found.");

        var course = instructor.Courses.FirstOrDefault()
                     ?? throw new InvalidOperationException($"No courses found for instructor {instructorEmail}.");

        var student = await dbContext.Students.FirstOrDefaultAsync()
                      ?? throw new InvalidOperationException("No students found in database.");

        var quizId = Guid.NewGuid();

        // Satisfy the Quiz domain rule: At least 3 questions, display order starting at 0, 1, 2
        var question1 = Essay.Create(Guid.NewGuid(), quizId, "Question 1Text", "Ref 1", 0, 10).Value;
        var question2 = Essay.Create(Guid.NewGuid(), quizId, "Question 2Text", "Ref 2", 1, 10).Value;

        var questionId = Guid.NewGuid();
        var question3 = Essay.Create(questionId, quizId, "Explain REST API principles in detail.", "Ref 3", 2, 10)
            .Value;

        var quiz = Quiz.Create(
            quizId,
            course.Id,
            instructor.Id,
            "Essay Quiz",
            DateTimeOffset.UtcNow.AddDays(1),
            DateTimeOffset.UtcNow.AddDays(2),
            [question1, question2, question3]).Value;

        var attemptId = Guid.NewGuid();
        var answerId = Guid.NewGuid();
        var answer = EssayAnswer.Create(
            answerId,
            student.Id,
            questionId,
            attemptId,
            "REST stands for Representational State Transfer...").Value;

        var attempt = QuizAttempt.Create(
            attemptId,
            student.Id,
            quizId,
            DateTime.UtcNow.AddMinutes(2),
            DateTime.UtcNow.AddMinutes(4),
            [answer]).Value;

        await dbContext.Quizzes.AddAsync(quiz);
        await dbContext.Questions.AddRangeAsync(new List<Question> { question1, question2, question3 });
        await dbContext.QuizAttempts.AddAsync(attempt);
        await dbContext.ManuallyGradedAnswers.AddAsync(answer);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        return (attemptId, answerId, questionId);
    }
}
