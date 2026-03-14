using QuizNova.Domain.Common;
using QuizNova.Domain.Common.Results;
using QuizNova.Domain.Entities.Quizzes.Questions.Base;

namespace QuizNova.Domain.Entities.Quizzes.Questions.Mcq.Choices;

public class Choice : AuditableEntity
{
    public Guid QuestionId { get; private set; }
    public string Text { get; private set; } = string.Empty;
    public int DisplayOrder { get; private set; }
    public Question? Question { get; private set; }

    private Choice()
    {
    }

    private Choice(Guid id, Guid questionId, string text, int displayOrder)
        : base(id)
    {
        QuestionId = questionId;
        Text = text;
        DisplayOrder = displayOrder;
    }

    public static Result<Choice> Create(Guid id, Guid questionId, string text, int displayOrder)
    {
        if (questionId == Guid.Empty)
        {
            return ChoiceErrors.QuestionIdRequired;
        }

        if (string.IsNullOrWhiteSpace(text))
        {
            return ChoiceErrors.TextRequired;
        }

        if (displayOrder < 0)
        {
            return ChoiceErrors.DisplayOrderInvalid;
        }

        return new Choice(id, questionId, text, displayOrder);
    }
}