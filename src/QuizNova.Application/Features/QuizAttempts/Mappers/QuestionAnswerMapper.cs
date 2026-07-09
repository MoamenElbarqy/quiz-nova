using QuizNova.Application.Features.QuizAttempts.DTOs;
using QuizNova.Domain.Entities.QuizAttempts.Answers.AutoGradedAnswers;
using QuizNova.Domain.Entities.QuizAttempts.Answers.AutoGradedAnswers.McqAnswer;
using QuizNova.Domain.Entities.QuizAttempts.Answers.AutoGradedAnswers.TrueFalseAnswer;
using QuizNova.Domain.Entities.QuizAttempts.Answers.Base;
using QuizNova.Domain.Entities.QuizAttempts.Answers.ManuallyGradedAnswers;
using QuizNova.Domain.Entities.QuizAttempts.Answers.ManuallyGradedAnswers.EssayAnswer;
using QuizNova.Domain.Entities.Quizzes.Questions.Base;

namespace QuizNova.Application.Features.QuizAttempts.Mappers;

public static class QuestionAnswerMapper
{
    public static QuestionAnswerDto ToDto(
        this QuestionAnswer answer,
        IReadOnlyDictionary<Guid, Question> questionsById)
    {
        questionsById.TryGetValue(answer.QuestionId, out var question);

        var questionText = question?.QuestionText ?? string.Empty;

        return answer switch
        {
            AutoGradedAnswer autoGraded => autoGraded.ToDto(questionText),
            ManuallyGradedAnswers manual => manual.ToDto(questionText),
            _ => throw new InvalidOperationException(
                $"Unknown answer type: {answer.GetType().Name}")
        };
    }

    private static AutoGradedAnswerDto ToDto(
        this AutoGradedAnswer answer,
        string questionText) => answer switch
        {
            McqAnswer mcq => mcq.ToDto(questionText),
            TfAnswer tf => tf.ToDto(questionText),
            _ => throw new InvalidOperationException(
                $"Unknown auto-graded answer type: {answer.GetType().Name}")
        };

    private static McqAnswerDto ToDto(this McqAnswer answer, string questionText) =>
        new(answer.Id,
            answer.QuestionId,
            questionText,
            "auto",
            "mcq",
            answer.IsCorrect,
            answer.SelectedChoiceId);

    private static TfAnswerDto ToDto(this TfAnswer answer, string questionText) =>
        new(answer.Id,
            answer.QuestionId,
            questionText,
            "auto",
            "tf",
            answer.IsCorrect,
            answer.StudentChoice);

    private static ManuallyGradedAnswerDto ToDto(
        this ManuallyGradedAnswers answer,
        string questionText) => answer switch
        {
            EssayAnswer essay => essay.ToDto(questionText),
            _ => throw new InvalidOperationException(
                $"Unknown manually graded answer type: {answer.GetType().Name}")
        };

    private static ManuallyGradedAnswerDto ToDto(this EssayAnswer answer, string questionText) =>
        new(answer.Id,
            answer.QuestionId,
            questionText,
            "manual",
            answer.Score,
            answer.StudentResponse,
            answer.Feedback);
}
