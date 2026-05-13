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

    public Guid QuizId { get; private set; }

    public string QuestionText { get; private set; } = string.Empty;

    public int DisplayOrder { get; private set; }

    public int Marks { get; private set; }

    public Quiz? Quiz { get; private set; }

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

