using QuizNova.Application.Features.Quizzes.Mappers;
using QuizNova.Tests.Common.Quizzes;

using Xunit;

namespace QuizNova.Application.UnitTests.Mappers;

public class StudentQuizMapperTests
{
    [Fact]
    public void ToStudentQuizDto_ShouldMapFieldsCorrectly()
    {
        // Arrange
        var quiz = QuizFactory.CreateQuiz().Value;

        // Act
        var dto = quiz.ToStudentQuizDto();

        // Assert
        Assert.NotNull(dto);
        Assert.Equal(quiz.Id, dto.QuizId);
        Assert.Equal(quiz.Title, dto.Title);
        Assert.Equal(quiz.Questions.Count(), dto.QuestionsCount);
        Assert.Equal(quiz.StartsAtUtc, dto.StartsAtUtc);
        Assert.Equal(quiz.EndsAtUtc, dto.EndsAtUtc);
        Assert.Equal(quiz.Status, dto.QuizStatus);

        // Graceful fallback since navigation property is null
        Assert.Equal(string.Empty, dto.CourseName);
}
}
