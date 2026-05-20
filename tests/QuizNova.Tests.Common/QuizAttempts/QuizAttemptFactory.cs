using QuizNova.Domain.Common.Results;
using QuizNova.Domain.Entities.QuizAttempts;
using QuizNova.Domain.Entities.QuizAttempts.Answers.Base;
using QuizNova.Tests.Common.QuizAttempts.Answers;

namespace QuizNova.Tests.Common.QuizAttempts;

public static class QuizAttemptFactory
{
    public static Result<QuizAttempt> CreateQuizAttempt(
        Guid? id = null,
        Guid? studentId = null,
        Guid? quizId = null,
        DateTime? startedAt = null,
        DateTime? submittedAt = null,
        List<QuestionAnswer>? studentAnswers = null)
    {
        var attemptId = id ?? Guid.NewGuid();
        var attemptStudentId = studentId ?? Guid.NewGuid();

        if (studentAnswers == null)
        {
            var defaultAnswer = AnswerFactory.CreateTfAnswer(
                studentId: attemptStudentId,
                quizAttemptId: attemptId).Value;
            studentAnswers = [defaultAnswer];
        }

        return QuizAttempt.Create(
            attemptId,
            attemptStudentId,
            quizId ?? Guid.NewGuid(),
            startedAt ?? DateTime.UtcNow.AddMinutes(-30),
            submittedAt ?? DateTime.UtcNow,
            studentAnswers);
    }
}
