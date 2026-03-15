using QuizNova.Domain.Common.Results;
using QuizNova.Domain.Entities.Quizzes.Questions.Base;

namespace QuizNova.Domain.Entities.Quizzes.Questions.Essay;

public sealed class EssayQuestion : Question
{
    private EssayQuestion()
    {
    }

    private EssayQuestion(
        Guid id,
        Guid quizId,
        string questionText,
        int displayOrder,
        int marks)
        : base(id, quizId, questionText, displayOrder, marks)
    {
    }

    public static Result<EssayQuestion> Create(
        Guid id,
        Guid quizId,
        string questionText,
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

        return new EssayQuestion(id, quizId, questionText, displayOrder, marks);
    }
}
