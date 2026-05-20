using QuizNova.Application.Features.QuizAttempts.Mappers;
using QuizNova.Tests.Common.QuizAttempts;
using QuizNova.Tests.Common.QuizAttempts.Answers;

using Xunit;

namespace QuizNova.Application.UnitTests.Mappers;

public class QuizAttemptMapperTests
{
    [Fact]
    public void ToQuizAttemptDto_ShouldMapFieldsCorrectly()
    {
        // Arrange
        var studentId = Guid.NewGuid();
        var attemptId = Guid.NewGuid();
        var quizId = Guid.NewGuid();

        var tfAnswer1 = AnswerFactory.CreateTfAnswer(
            studentId: studentId,
            questionId: Guid.NewGuid(),
            quizAttemptId: attemptId,
            studentChoice: true,
            isCorrect: true).Value;

        var tfAnswer2 = AnswerFactory.CreateTfAnswer(
            studentId: studentId,
            questionId: Guid.NewGuid(),
            quizAttemptId: attemptId,
            studentChoice: false,
            isCorrect: true).Value;

        var mcqAnswer = AnswerFactory.CreateMcqAnswer(
            studentId: studentId,
            questionId: Guid.NewGuid(),
            quizAttemptId: attemptId,
            selectedChoiceId: Guid.NewGuid(),
            isCorrect: false).Value;

        var quizAttempt = QuizAttemptFactory.CreateQuizAttempt(
            id: attemptId,
            studentId: studentId,
            quizId: quizId,
            studentAnswers: [tfAnswer1, tfAnswer2, mcqAnswer]).Value;

        // Act
        var dto = quizAttempt.ToQuizAttemptDto();

        // Assert
        Assert.NotNull(dto);
        Assert.Equal(quizAttempt.Id, dto.QuizAttemptId);
        Assert.Equal(quizAttempt.QuizId, dto.QuizId);
        Assert.Equal(quizAttempt.StartedAt, dto.StartedAt);
        Assert.Equal(quizAttempt.SubmittedAt, dto.SubmittedAt);

        // Verify default / fallback mappings for null Quiz and Question navigation properties
        Assert.Equal(string.Empty, dto.QuizTitle);
        Assert.Equal(0, dto.TotalQuestions);
        Assert.Equal(3, dto.AnsweredQuestions);
        Assert.Equal(2, dto.CorrectAnswers); // TF1 and TF2 are correct
        Assert.Equal(0, dto.Score); // Score calculation relies on autoGradedAnswer.Question which is null
        Assert.Empty(dto.Questions);

        // Check mapped answers
        Assert.NotNull(dto.Answers);
        Assert.Equal(3, dto.Answers.Count);

        var mappedTfAnswer1 = dto.Answers.FirstOrDefault(a => a.QuestionId == tfAnswer1.QuestionId);
        Assert.NotNull(mappedTfAnswer1);
        Assert.Equal("tf", mappedTfAnswer1.AnswerType);
        Assert.True(mappedTfAnswer1.IsCorrect);
        Assert.Equal(string.Empty, mappedTfAnswer1.QuestionText);

        var mappedMcqAnswer = dto.Answers.FirstOrDefault(a => a.QuestionId == mcqAnswer.QuestionId);
        Assert.NotNull(mappedMcqAnswer);
        Assert.Equal("mcq", mappedMcqAnswer.AnswerType);
        Assert.False(mappedMcqAnswer.IsCorrect);
        Assert.Equal(string.Empty, mappedMcqAnswer.QuestionText);
}
}
