using QuizNova.Domain.Common;
using QuizNova.Domain.Common.Results;
using QuizNova.Domain.Entities.Quizzes.Questions.Base;
using QuizNova.Domain.Entities.Quizzes.Questions.MatchingPairs;

namespace QuizNova.Domain.Entities.Quizzes.Questions.MatchingPairs;

public class MatchingQuestion : Question
{
    public string Prompt { get; private set; } = string.Empty;

    public List<MatchingLeftChoice> LeftChoices { get; private set; } = [];

    public List<MatchingRightChoice> RightChoices { get; private set; } = [];

    public List<MatchingCorrectPair> CorrectPairs { get; private set; } = [];

    private MatchingQuestion()
    {
    }

    private MatchingQuestion(Guid id, Guid quizId, string prompt, int displayOrder, int points)
        : base(id, quizId, displayOrder, points)
    {
        Prompt = prompt;
    }

    public static Result<MatchingQuestion> Create(
        Guid id,
        Guid quizId,
        string prompt,
        int displayOrder,
        int points)
    {
        if (quizId == Guid.Empty)
        {
            return MatchingQuestionErrors.QuizIdRequired;
        }

        if (string.IsNullOrWhiteSpace(prompt))
        {
            return MatchingQuestionErrors.PromptRequired;
        }

        if (displayOrder < 0)
        {
            return MatchingQuestionErrors.DisplayOrderInvalid;
        }

        if (points <= 0)
        {
            return MatchingQuestionErrors.PointsInvalid;
        }

        return new MatchingQuestion(id, quizId, prompt, displayOrder, points);
    }
}
