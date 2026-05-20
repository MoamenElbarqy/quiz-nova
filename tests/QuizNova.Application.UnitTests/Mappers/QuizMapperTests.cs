using QuizNova.Application.Features.Quizzes.Mappers;
using QuizNova.Tests.Common.Quizzes;
using QuizNova.Tests.Common.Quizzes.Questions;

using Xunit;

namespace QuizNova.Application.UnitTests.Mappers;

public class QuizMapperTests
{
    [Fact]
    public void ToQuizDto_ShouldMapFieldsCorrectly()
    {
        // Arrange
        var quizId = Guid.NewGuid();
        var q1 = QuestionFactory.CreateTfQuestion(quizId: quizId, displayOrder: 1, marks: 5).Value;
        var q2 = QuestionFactory.CreateTfQuestion(quizId: quizId, displayOrder: 2, marks: 15).Value;
        var q3 = QuestionFactory.CreateTfQuestion(quizId: quizId, displayOrder: 3, marks: 10).Value;

        var quiz = QuizFactory.CreateQuiz(
            id: quizId,
            title: "Advanced C# Quiz",
            questions: [q1, q2, q3]).Value;

        // Act
        var dto = quiz.ToQuizDto();

        // Assert
        Assert.NotNull(dto);
        Assert.Equal(quiz.Id, dto.QuizId);
        Assert.Equal(quiz.Title, dto.Title);
        Assert.Equal(string.Empty, dto.CourseName);
        Assert.Equal(string.Empty, dto.InstructorName);
        Assert.Equal(30, dto.Marks); // 5 + 15 + 10
        Assert.Equal(quiz.CourseId, dto.CourseId);
        Assert.Equal(quiz.InstructorId, dto.InstructorId);
        Assert.Equal(quiz.StartsAtUtc, dto.StartsAtUtc);
        Assert.Equal(quiz.EndsAtUtc, dto.EndsAtUtc);

        // Assert questions mapped
        Assert.NotNull(dto.Questions);
        Assert.Equal(3, dto.Questions.Count);
        Assert.Equal(q1.Id, dto.Questions.ElementAt(0).Id);
        Assert.Equal(q2.Id, dto.Questions.ElementAt(1).Id);
        Assert.Equal(q3.Id, dto.Questions.ElementAt(2).Id);
    }

    [Fact]
    public void ToQuizDto_StateTransitions_ShouldBeCorrect()
    {
        // Assert
        // Upcoming Quiz
        var upcomingQuiz = QuizFactory.CreateQuiz(
            startsAtUtc: DateTimeOffset.UtcNow.AddMinutes(10),
            endsAtUtc: DateTimeOffset.UtcNow.AddMinutes(30)).Value;
        Assert.Equal("Upcoming", upcomingQuiz.ToQuizDto().State);

        // Active Quiz
        var activeQuiz = QuizFactory.CreateQuiz(
            startsAtUtc: DateTimeOffset.UtcNow.AddMinutes(-10),
            endsAtUtc: DateTimeOffset.UtcNow.AddMinutes(10)).Value;
        Assert.Equal("Active", activeQuiz.ToQuizDto().State);

        // Completed Quiz
        var completedQuiz = QuizFactory.CreateQuiz(
            startsAtUtc: DateTimeOffset.UtcNow.AddMinutes(-30),
            endsAtUtc: DateTimeOffset.UtcNow.AddMinutes(-10)).Value;
        Assert.Equal("Completed", completedQuiz.ToQuizDto().State);
    }
}
