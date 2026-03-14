using QuizNova.Domain.Common;
using QuizNova.Domain.Common.Results;

namespace QuizNova.Domain.Entities.Quizzes.Questions.MatchingPairs;

public class MatchingRightChoice : AuditableEntity
{
    public Guid QuestionId { get; private set; }

    public string ChoiceText { get; private set; } = string.Empty;

    public MatchingQuestion? Question { get; private set; }

    private MatchingRightChoice()
    {
    }

    private MatchingRightChoice(Guid id, Guid questionId, string choiceText)
        : base(id)
    {
        QuestionId = questionId;
        ChoiceText = choiceText;
    }

    public static Result<MatchingRightChoice> Create(Guid id, Guid questionId, string choiceText)
    {
        if (questionId == Guid.Empty)
        {
            return MatchingRightChoiceErrors.QuestionIdRequired;
        }

        if (string.IsNullOrWhiteSpace(choiceText))
        {
            return MatchingRightChoiceErrors.ChoiceTextRequired;
        }

        return new MatchingRightChoice(id, questionId, choiceText);
    }
}
