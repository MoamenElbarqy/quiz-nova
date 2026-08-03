using QuizNova.Domain.Common.Results;
using QuizNova.Domain.Entities.QuizAttempts;

namespace QuizNova.Tests.Common.QuizAttempts;

public static class QuizAttemptFactory
{
    public static Result<QuizAttempt> CreateQuizAttempt(
        Guid? quizId = null,
        Guid? id = null,
        Guid? studentId = null,
        DateTimeOffset? quizEndsAtUtc = null)
    {
        return QuizAttempt.Start(
            id ?? Guid.NewGuid(),
            studentId ?? Guid.NewGuid(),
            quizId ?? Guid.NewGuid(),
            DateTime.UtcNow,
            quizEndsAtUtc ?? DateTimeOffset.UtcNow.AddHours(2));
    }
}
