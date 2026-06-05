using QuizNova.Domain.Common.Results;
using QuizNova.Domain.Entities.QuizAttempts;
using QuizNova.Domain.Entities.QuizAttempts.Answers.Base;

namespace QuizNova.Tests.Common.QuizAttempts;

public static class QuizAttemptFactory
{
    public static Result<QuizAttempt> CreateQuizAttempt(
        Guid? quizId = null,
        DateTime? startedAt = null,
        DateTime? submittedAt = null,
        Guid? id = null,
        Guid? studentId = null,
        List<QuestionAnswer>? studentAnswers = null)
    {
        var attemptId = id ?? Guid.NewGuid();
        var attemptStudentId = studentId ?? Guid.NewGuid();

        return QuizAttempt.Create(
            attemptId,
            attemptStudentId,
            quizId ?? Guid.NewGuid(),
            startedAt ?? DateTime.UtcNow.AddMinutes(-30),
            submittedAt ?? DateTime.UtcNow,
            studentAnswers ?? []);
    }
}
