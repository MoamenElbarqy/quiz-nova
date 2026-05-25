using System.Diagnostics.CodeAnalysis;

using QuizNova.Domain.Common.Results;
using QuizNova.Domain.Entities.QuizAttempts.Answers.Base;
using QuizNova.Domain.Entities.QuizAttempts.Answers.ManuallyGradedAnswers.EssayAnswer;

namespace QuizNova.Domain.Entities.Quizzes.Questions.ManuallyGradedQuestions;

public class Essay : ManuallyGradedQuestion<string>
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

        if (answerReference is not null)
        {
            var trimmedReference = answerReference.Trim();

            if (string.IsNullOrWhiteSpace(trimmedReference))
            {
                return EssayErrors.AnswerReferenceRequired;
            }

            if (trimmedReference.Length < 3)
            {
                return EssayErrors.AnswerReferenceTooShort;
            }

            if (trimmedReference.Length > 1000)
            {
                return EssayErrors.AnswerReferenceTooLong;
            }

            answerReference = trimmedReference;
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

        if (answerReference is not null)
        {
            var trimmedReference = answerReference.Trim();

            if (string.IsNullOrWhiteSpace(trimmedReference))
            {
                return EssayErrors.AnswerReferenceRequired;
            }

            if (trimmedReference.Length < 3)
            {
                return EssayErrors.AnswerReferenceTooShort;
            }

            if (trimmedReference.Length > 1000)
            {
                return EssayErrors.AnswerReferenceTooLong;
            }

            answerReference = trimmedReference;
        }

        AnswerReference = answerReference;

        return Result.Updated;
    }

    public override Result<QuestionAnswer> Solve(string answer, Guid studentId, Guid quizAttemptId)
    {
        var createResult = EssayAnswer.Create(
            Guid.NewGuid(),
            studentId,
            Id,
            quizAttemptId,
            answer);

        if (createResult.IsError)
        {
            return createResult.TopError;
        }

        return createResult.Value;
    }
}
