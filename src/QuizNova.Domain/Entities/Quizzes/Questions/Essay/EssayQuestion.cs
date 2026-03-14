using QuizNova.Domain.Common.Results;
using QuizNova.Domain.Entities.Quizzes.Questions.Base;

namespace QuizNova.Domain.Entities.Quizzes.Questions.Essay;

public class EssayQuestion : Question
{
    public string Prompt { get; private set; } = string.Empty;

    private EssayQuestion()
    {
    }

    private EssayQuestion(Guid id, Guid quizId, string prompt, int displayOrder, int points)
        : base(id, quizId, displayOrder, points)
    {
        Prompt = prompt;
    }

    public static Result<EssayQuestion> Create(
        Guid id,
        Guid quizId,
        string prompt,
        int displayOrder,
        int points)
    {
        if (quizId == Guid.Empty)
        {
            return EssayQuestionErrors.QuizIdRequired;
        }

        if (string.IsNullOrWhiteSpace(prompt))
        {
            return EssayQuestionErrors.PromptRequired;
        }

        if (displayOrder < 0)
        {
            return EssayQuestionErrors.DisplayOrderInvalid;
        }

        if (points <= 0)
        {
            return EssayQuestionErrors.PointsInvalid;
        }

        return new EssayQuestion(id, quizId, prompt, displayOrder, points);
    }
}
