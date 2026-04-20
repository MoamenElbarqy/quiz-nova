using QuizNova.Domain.Common.Results;
using QuizNova.Domain.Entities.Quizzes.Questions.Base;

namespace QuizNova.Domain.Entities.Quizzes.Questions.TrueFalse;

public class TrueFalseQuestion : Question
{
    public bool CorrectChoice { get; private set; }

    private TrueFalseQuestion()
    {
    }

    private TrueFalseQuestion(
        Guid id,
        Guid quizId,
        string questionText,
        bool correctChoice,
        int displayOrder,
        int marks)
        : base(id, quizId, questionText, displayOrder, marks)
    {
        CorrectChoice = correctChoice;
    }

    public static Result<TrueFalseQuestion> Create(
        Guid id,
        Guid quizId,
        string questionText,
        bool correctChoice,
        int displayOrder,
        int marks)
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

        return new TrueFalseQuestion(
            id,
            quizId,
            questionText,
            correctChoice,
            displayOrder,
            marks);
    }
}
