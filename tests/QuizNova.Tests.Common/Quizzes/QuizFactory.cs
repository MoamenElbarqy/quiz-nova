using QuizNova.Domain.Common.Results;
using QuizNova.Domain.Entities.Quizzes;
using QuizNova.Domain.Entities.Quizzes.Questions;

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
        IEnumerable<CreateQuestionArgs>? questionArgs = null)
    {
        var quizId = id ?? Guid.NewGuid();
        questionArgs ??= [
            new CreateTfArgs("Question 1?", 10, true),
            new CreateTfArgs("Question 2?", 10, false),
            new CreateTfArgs("Question 3?", 10, true),
        ];

        return Quiz.Create(
            quizId,
            courseId ?? Guid.NewGuid(),
            instructorId ?? Guid.NewGuid(),
            title,
            startsAtUtc ?? DateTimeOffset.UtcNow.AddHours(1),
            endsAtUtc ?? DateTimeOffset.UtcNow.AddHours(3),
            questionArgs);
    }
}
