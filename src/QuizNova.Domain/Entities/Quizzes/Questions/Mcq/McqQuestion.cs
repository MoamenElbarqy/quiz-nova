using QuizNova.Domain.Common.Results;
using QuizNova.Domain.Entities.Quizzes.Questions.Base;
using QuizNova.Domain.Entities.Quizzes.Questions.Mcq.Choices;

namespace QuizNova.Domain.Entities.Quizzes.Questions.Mcq;

public class McqQuestion : Question
{
    private readonly List<Choice> _choices;

    private McqQuestion()
    {
        _choices = new List<Choice>();
    }

    private McqQuestion(
        Guid id,
        Guid quizId,
        string questionText,
        Guid correctChoiceId,
        int displayOrder,
        int marks,
        List<Choice> choices)
        : base(id, quizId, questionText, displayOrder, marks)
    {
        CorrectChoiceId = correctChoiceId;
        _choices = choices;
    }

    public int NumberOfChoices => Choices.Count();

    public Guid CorrectChoiceId { get; private set; }

    public Choice? CorrectChoice { get; private set; }

    public IEnumerable<Choice> Choices => _choices.AsReadOnly();

    public static Result<McqQuestion> Create(
        Guid id,
        Guid quizId,
        string questionText,
        int numberOfChoices,
        Guid correctChoiceId,
        int displayOrder,
        int marks,
        List<Choice> choices)
    {
        var validationError = ValidateCommon(
            quizId,
            questionText,
            displayOrder,
            marks);

        if (validationError.IsError)
        {
            return validationError.TopError;
        }

        if (numberOfChoices < 2)
        {
            return McqQuestionErrors.NumberOfChoicesInvalid;
        }

        if (correctChoiceId == Guid.Empty)
        {
            return McqQuestionErrors.CorrectChoiceIdRequired;
        }

        return new McqQuestion(
            id,
            quizId,
            questionText,
            correctChoiceId,
            displayOrder,
            marks,
            choices);
    }
}
