using QuizNova.Domain.Common;
using QuizNova.Domain.Common.Results;
using QuizNova.Domain.Entities.Quizzes.Questions.Base;

namespace QuizNova.Domain.Entities.Quizzes.Questions.TrueFalse;

public class TrueFalseQuestion : Question
{
    public string Statement { get; private set; } = string.Empty;

    public bool CorrectChoice { get; private set; }

    private TrueFalseQuestion()
    {
    }

    private TrueFalseQuestion(
        Guid id,
        Guid quizId,
        string statement,
        bool correctChoice,
        int displayOrder,
        int points)
        : base(id, quizId, displayOrder, points)
    {
        Statement = statement;
        CorrectChoice = correctChoice;
    }

    public static Result<TrueFalseQuestion> Create(
        Guid id,
        Guid quizId,
        string statement,
        bool correctChoice,
        int displayOrder,
        int points)
    {
        if (quizId == Guid.Empty)
        {
            return TrueFalseQuestionErrors.QuizIdRequired;
        }

        if (string.IsNullOrWhiteSpace(statement))
        {
            return TrueFalseQuestionErrors.StatementRequired;
        }

        if (displayOrder < 0)
        {
            return TrueFalseQuestionErrors.DisplayOrderInvalid;
        }

        if (points <= 0)
        {
            return TrueFalseQuestionErrors.PointsInvalid;
        }

        return new TrueFalseQuestion(id, quizId, statement, correctChoice, displayOrder, points);
    }
}
