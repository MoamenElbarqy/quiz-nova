using QuizNova.Domain.Common.Results;
using QuizNova.Domain.Entities.QuizAttempts.Answers.Base;
using QuizNova.Domain.Entities.Quizzes.Questions.TrueFalse;

namespace QuizNova.Domain.Entities.QuizAttempts.Answers.TrueFalseAnswer;

public class TfAnswer : QuestionAnswer
{
    public bool StudentChoice { get; }

    public Tf? Tf { get; private set; }

    public override bool IsCorrect => Tf is not null && Tf.CorrectChoice == StudentChoice;

    // Required by EF Core
    private TfAnswer()
        : base(
            Guid.Empty,
            Guid.Empty,
            Guid.Empty,
            Guid.Empty)
    {
    }

    private TfAnswer(
        Guid id,
        Guid studentId,
        Guid questionId,
        Guid quizAttemptId,
        bool studentChoice)
        : base(id, studentId, questionId, quizAttemptId)
    {
        StudentChoice = studentChoice;
    }

    public static Result<TfAnswer> Create(
        Guid id,
        Guid studentId,
        Guid questionId,
        Guid quizAttemptId,
        bool studentChoice)
    {
        var commonValidationError = ValidateCommon(studentId, questionId, quizAttemptId);

        if (commonValidationError.IsError)
        {
            return commonValidationError.TopError;
        }

        return new TfAnswer(
            id,
            studentId: studentId,
            questionId,
            quizAttemptId,
            studentChoice);
    }
}
