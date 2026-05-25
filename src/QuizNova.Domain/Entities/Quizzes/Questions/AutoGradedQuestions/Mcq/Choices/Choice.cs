using System.Diagnostics.CodeAnalysis;

using QuizNova.Domain.Common;
using QuizNova.Domain.Common.Results;
using QuizNova.Domain.Entities.Quizzes.Questions.Base;

namespace QuizNova.Domain.Entities.Quizzes.Questions.AutoGradedQuestions.Mcq.Choices;

public class Choice : Entity
{
    public Guid QuestionId { get; private set; }

    public required string Text { get; set; }

    public int DisplayOrder { get; private set; }

    public Question? Question { get; init; }

    [SetsRequiredMembers]
    private Choice()
    {
    }

    [SetsRequiredMembers]
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

        var trimmedText = text.Trim();

        if (trimmedText.Length < 3)
        {
            return ChoiceErrors.ChoiceTooShort;
        }

        if (trimmedText.Length > 100)
        {
            return ChoiceErrors.ChoiceTooLong;
        }

        if (displayOrder < 0)
        {
            return ChoiceErrors.DisplayOrderInvalid;
        }

        return new Choice(id, questionId, text, displayOrder);
    }

    internal Result<Updated> Update(string text, int displayOrder)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return ChoiceErrors.TextRequired;
        }

        var trimmedText = text.Trim();

        if (trimmedText.Length < 3)
        {
            return ChoiceErrors.ChoiceTooShort;
        }

        if (trimmedText.Length > 100)
        {
            return ChoiceErrors.ChoiceTooLong;
        }

        if (displayOrder < 0)
        {
            return ChoiceErrors.DisplayOrderInvalid;
        }

        Text = text;
        DisplayOrder = displayOrder;

        return Result.Updated;
    }
}
