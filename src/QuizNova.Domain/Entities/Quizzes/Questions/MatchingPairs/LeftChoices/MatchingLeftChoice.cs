using QuizNova.Domain.Common;
using QuizNova.Domain.Common.Results;

namespace QuizNova.Domain.Entities.Quizzes.Questions.MatchingPairs.LeftChoices;

public class MatchingLeftChoice : AuditableEntity
{
    private MatchingLeftChoice()
    {
    }

    private MatchingLeftChoice(Guid id, Guid questionId, string choiceText)
        : base(id)
    {
        QuestionId = questionId;
        ChoiceText = choiceText;
    }

    public Guid QuestionId { get; private set; }

    public string ChoiceText { get; private set; } = string.Empty;

    public MatchingQuestion? Question { get; private set; }

    public static Result<MatchingLeftChoice> Create(
        Guid id,
        Guid questionId,
        string choiceText)
    {
        if (questionId == Guid.Empty)
        {
            return MatchingLeftChoiceErrors.QuestionIdRequired;
        }

        if (string.IsNullOrWhiteSpace(choiceText))
        {
            return MatchingLeftChoiceErrors.ChoiceTextRequired;
        }

        return new MatchingLeftChoice(id, questionId, choiceText);
    }
}
