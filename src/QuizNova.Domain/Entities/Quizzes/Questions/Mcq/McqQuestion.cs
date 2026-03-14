using QuizNova.Domain.Common.Results;
using QuizNova.Domain.Entities.Quizzes.Questions.Base;

namespace QuizNova.Domain.Entities.Quizzes.Questions.Mcq;

public class McqQuestion : Question
{
    public string Prompt { get; private set; } = string.Empty;

    public int NumberOfChoices { get; private set; }

    public Guid CorrectChoiceId { get; private set; }

    public Choices.Choice? CorrectChoice { get; private set; }

    public List<Choices.Choice> Choices { get; private set; } = [];

    private McqQuestion()
    {
    }

    private McqQuestion(
        Guid id,
        Guid quizId,
        string prompt,
        int numberOfChoices,
        Guid correctChoiceId,
        int displayOrder,
        int points)
        : base(id, quizId, displayOrder, points)
    {
        Prompt = prompt;
        NumberOfChoices = numberOfChoices;
        CorrectChoiceId = correctChoiceId;
    }

    public static Result<McqQuestion> Create(
        Guid id,
        Guid quizId,
        string prompt,
        int numberOfChoices,
        Guid correctChoiceId,
        int displayOrder,
        int points)
    {
        if (quizId == Guid.Empty)
        {
            return McqQuestionErrors.QuizIdRequired;
        }

        if (string.IsNullOrWhiteSpace(prompt))
        {
            return McqQuestionErrors.PromptRequired;
        }

        if (numberOfChoices < 2)
        {
            return McqQuestionErrors.NumberOfChoicesInvalid;
        }

        if (correctChoiceId == Guid.Empty)
        {
            return McqQuestionErrors.CorrectChoiceIdRequired;
        }

        if (displayOrder < 0)
        {
            return McqQuestionErrors.DisplayOrderInvalid;
        }

        if (points <= 0)
        {
            return McqQuestionErrors.PointsInvalid;
        }

        return new McqQuestion(id, quizId, prompt, numberOfChoices, correctChoiceId, displayOrder, points);
    }
}
