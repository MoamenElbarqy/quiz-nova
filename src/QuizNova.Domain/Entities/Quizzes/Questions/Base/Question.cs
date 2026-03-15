using QuizNova.Domain.Common;
using QuizNova.Domain.Common.Results;

namespace QuizNova.Domain.Entities.Quizzes.Questions.Base;

public abstract class Question : AuditableEntity
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

    public Guid QuizId { get; protected set; }

    public string QuestionText { get; protected set; } = string.Empty;

    public int DisplayOrder { get; protected set; }

    public int Marks { get; protected set; }

    public Quiz? Quiz { get; protected set; }

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
