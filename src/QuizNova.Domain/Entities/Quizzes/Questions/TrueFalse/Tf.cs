using QuizNova.Domain.Common.Results;
using QuizNova.Domain.Entities.Quizzes.Questions.Base;

namespace QuizNova.Domain.Entities.Quizzes.Questions.TrueFalse;

public class Tf : Question
{
    public bool CorrectChoice { get; private set; }

    private Tf()
    {
    }

    private Tf(
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

    public static Result<Tf> Create(
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

        return new Tf(
            id,
            quizId,
            questionText,
            correctChoice,
            displayOrder,
            marks);
    }

    internal Result<Updated> Update(
        string questionText,
        int displayOrder,
        int marks,
        bool correctChoice)
    {
        var baseResult = UpdateBase(questionText, displayOrder, marks);

        if (baseResult.IsError)
        {
            return baseResult.TopError;
        }

        CorrectChoice = correctChoice;

        return Result.Updated;
    }
}

