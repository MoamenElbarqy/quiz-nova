using QuizNova.Domain.Common;
using QuizNova.Domain.Common.Results;

namespace QuizNova.Domain.Entities.Quizzes.Questions.MatchingPairs;

public class MatchingCorrectPair : AuditableEntity
{
    public Guid QuestionId { get; private set; }

    public Guid LeftChoiceId { get; private set; }

    public Guid RightChoiceId { get; private set; }

    public MatchingQuestion? Question { get; private set; }

    private MatchingCorrectPair()
    {
    }

    private MatchingCorrectPair(Guid id, Guid questionId, Guid leftChoiceId, Guid rightChoiceId)
        : base(id)
    {
        QuestionId = questionId;
        LeftChoiceId = leftChoiceId;
        RightChoiceId = rightChoiceId;
    }

    public static Result<MatchingCorrectPair> Create(
        Guid id,
        Guid questionId,
        Guid leftChoiceId,
        Guid rightChoiceId)
    {
        if (questionId == Guid.Empty)
        {
            return MatchingCorrectPairErrors.QuestionIdRequired;
        }

        if (leftChoiceId == Guid.Empty)
        {
            return MatchingCorrectPairErrors.LeftChoiceIdRequired;
        }

        if (rightChoiceId == Guid.Empty)
        {
            return MatchingCorrectPairErrors.RightChoiceIdRequired;
        }

        return new MatchingCorrectPair(id, questionId, leftChoiceId, rightChoiceId);
    }
}
