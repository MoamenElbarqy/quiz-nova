using QuizNova.Domain.Common;
using QuizNova.Domain.Common.Results;

namespace QuizNova.Domain.Entities.Quizzes.Questions.MatchingPairs.CorrectAnswerPairs;

public class CorrectAnswerPair : AuditableEntity
{
    private CorrectAnswerPair()
    {
    }

    private CorrectAnswerPair(
        Guid id,
        Guid questionId,
        Guid leftChoiceId,
        Guid rightChoiceId)
        : base(id)
    {
        QuestionId = questionId;
        LeftChoiceId = leftChoiceId;
        RightChoiceId = rightChoiceId;
    }

    public Guid QuestionId { get; private set; }

    public Guid LeftChoiceId { get; private set; }

    public Guid RightChoiceId { get; private set; }

    public MatchingQuestion? Question { get; private set; }

    public static Result<CorrectAnswerPair> Create(
        Guid id,
        Guid questionId,
        Guid leftChoiceId,
        Guid rightChoiceId)
    {
        if (questionId == Guid.Empty)
        {
            return CorrectAnswerPairErros.QuestionIdRequired;
        }

        if (leftChoiceId == Guid.Empty)
        {
            return CorrectAnswerPairErros.LeftChoiceIdRequired;
        }

        if (rightChoiceId == Guid.Empty)
        {
            return CorrectAnswerPairErros.RightChoiceIdRequired;
        }

        return new CorrectAnswerPair(id, questionId, leftChoiceId, rightChoiceId);
    }
}
