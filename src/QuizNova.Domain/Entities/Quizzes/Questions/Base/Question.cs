using System.Diagnostics.CodeAnalysis;

using QuizNova.Domain.Common;
using QuizNova.Domain.Common.Results;
using QuizNova.Domain.Entities.QuizAttempts.Answers.Base;

namespace QuizNova.Domain.Entities.Quizzes.Questions.Base;

public abstract class Question : Entity
{
    [SetsRequiredMembers]
    protected Question()
    {
    }

    [SetsRequiredMembers]
    protected Question(
        Guid id,
        Guid quizId,
        string questionText,
        int displayOrder,
        int marks)
        : base(id)
    {
        QuizId = quizId;
        QuestionText = questionText;
        DisplayOrder = displayOrder;
        Marks = marks;
    }

    public Guid QuizId { get; private set; }

    public required string QuestionText { get; set; }

    public int DisplayOrder { get; private set; }

    public int Marks { get; private set; }

    public Quiz? Quiz { get; init; }

    internal Result<Updated> UpdateBase(
        string questionText,
        int displayOrder,
        int marks)
    {
        var validation = ValidateCommon(QuizId, questionText, displayOrder, marks);

        if (validation.IsError)
        {
            return validation.TopError;
        }

        QuestionText = questionText;
        DisplayOrder = displayOrder;
        Marks = marks;

        return Result.Updated;
    }

    protected static Result<Validated> ValidateCommon(
        Guid quizId,
        string questionText,
        int displayOrder,
        int marks)
    {
        if (quizId == Guid.Empty)
        {
            return QuestionErrors.QuizIdRequired;
        }

        if (string.IsNullOrWhiteSpace(questionText))
        {
            return QuestionErrors.QuestionTextRequired;
        }

        if (displayOrder < 0)
        {
            return QuestionErrors.DisplayOrderInvalid;
        }

        if (marks <= 0)
        {
            return QuestionErrors.MarksInvalid;
        }

        return Result.Validated;
    }
}

public abstract class Question<TAnswer> : Question
{
    [SetsRequiredMembers]
    protected Question()
    {
    }

    [SetsRequiredMembers]
    protected Question(
        Guid id,
        Guid quizId,
        string questionText,
        int displayOrder,
        int marks)
        : base(id, quizId, questionText, displayOrder, marks)
    {
    }

    public abstract Result<QuestionAnswer> Solve(TAnswer answer, Guid studentId, Guid quizAttemptId);
}
