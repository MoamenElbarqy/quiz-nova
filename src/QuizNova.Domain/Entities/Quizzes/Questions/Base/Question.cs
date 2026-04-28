using QuizNova.Domain.Common;
using QuizNova.Domain.Common.Results;

namespace QuizNova.Domain.Entities.Quizzes.Questions.Base;

public abstract class Question : Entity
{
    protected Question()
    {
    }

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

    public Guid QuizId { get; protected init; }

    public string QuestionText { get; protected init; } = string.Empty;

    public int DisplayOrder { get; protected init; }

    public int Marks { get; protected init; }

    public Quiz? Quiz { get; protected init; }

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
