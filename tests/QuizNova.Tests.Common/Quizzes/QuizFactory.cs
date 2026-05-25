using QuizNova.Domain.Common.Results;
using QuizNova.Domain.Entities.Quizzes;
using QuizNova.Domain.Entities.Quizzes.Questions.AutoGradedQuestions.TrueFalse;
using QuizNova.Domain.Entities.Quizzes.Questions.Base;

namespace QuizNova.Tests.Common.Quizzes;

public static class QuizFactory
{
    public static Result<Quiz> CreateQuiz(
        Guid? id = null,
        Guid? courseId = null,
        Guid? instructorId = null,
        string title = "Test Quiz",
        DateTimeOffset? startsAtUtc = null,
        DateTimeOffset? endsAtUtc = null,
        List<Question>? questions = null)
    {
        var quizId = id ?? Guid.NewGuid();
        if (questions == null)
        {
            var q1 = Tf.Create(
                Guid.NewGuid(),
                quizId,
                "Question 1?",
                true,
                0,
                10).Value;
            var q2 = Tf.Create(
                Guid.NewGuid(),
                quizId,
                "Question 2?",
                false,
                1,
                10).Value;
            var q3 = Tf.Create(
                Guid.NewGuid(),
                quizId,
                "Question 3?",
                true,
                2,
                10).Value;
            questions = [q1, q2, q3];
        }

        return Quiz.Create(
            quizId,
            courseId ?? Guid.NewGuid(),
            instructorId ?? Guid.NewGuid(),
            title,
            startsAtUtc ?? DateTimeOffset.UtcNow.AddHours(1),
            endsAtUtc ?? DateTimeOffset.UtcNow.AddHours(3),
            questions);
    }
}
