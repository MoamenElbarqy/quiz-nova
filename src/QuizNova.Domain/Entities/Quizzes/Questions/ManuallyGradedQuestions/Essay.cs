using System.Diagnostics.CodeAnalysis;

using QuizNova.Domain.Common.Results;

namespace QuizNova.Domain.Entities.Quizzes.Questions.ManuallyGradedQuestions;

public class Essay : ManuallyGradedQuestion
{
    public string? AnswerReference { get; private set; }

    // Required by Entity Framework Core
    [SetsRequiredMembers]
    private Essay()
    {
    }

    [SetsRequiredMembers]
    private Essay(
        Guid id,
        Guid quizId,
        string questionText,
        string? answerReference,
        int displayOrder,
        int marks)
        : base(id, quizId, questionText, displayOrder, marks)
    {
        AnswerReference = answerReference;
    }

    public static Result<Essay> Create(
        Guid id,
        Guid quizId,
        string questionText,
        string? answerReference,
        int displayOrder,
        int marks)
    {
        var validationError = ValidateCommon(
            quizId,
            questionText,
            displayOrder,
            marks);

        if (validationError.IsError)
        {
            return validationError.TopError;
        }

        if (answerReference is not null && string.IsNullOrWhiteSpace(answerReference))
        {
            return EssayErrors.AnswerReferenceRequired;
        }

        return new Essay(
            id,
            quizId,
            questionText,
            answerReference,
            displayOrder,
            marks);
    }

    public Result<Updated> Update(
        string questionText,
        int displayOrder,
        int marks,
        string? answerReference)
    {
        var baseResult = UpdateBase(questionText, displayOrder, marks);

        if (baseResult.IsError)
        {
            return baseResult.TopError;
        }

        if (answerReference is not null && string.IsNullOrWhiteSpace(answerReference))
        {
            return EssayErrors.AnswerReferenceRequired;
        }

        AnswerReference = answerReference;

        return Result.Updated;
    }
}
