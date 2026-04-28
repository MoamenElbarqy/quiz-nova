using QuizNova.Domain.Common;
using QuizNova.Domain.Common.Results;
using QuizNova.Domain.Entities.Quizzes.Questions.Base;
using QuizNova.Domain.Entities.Users.Student;

namespace QuizNova.Domain.Entities.QuizAttempts.Answers.Base;

public abstract class QuestionAnswer : Entity
{
    public Guid StudentId { get; private set; }

    public Guid QuestionId { get; private set; }

    public Guid QuizAttemptId { get; private set; }

    public QuizAttempt? QuizAttempt { get; init; }

    public Student? Student { get; init; }

    public Question? Question { get; init; }

    public abstract bool IsCorrect { get; }

    protected QuestionAnswer(
        Guid id,
        Guid studentId,
        Guid questionId,
        Guid quizAttemptId)
        : base(id)
    {
        StudentId = studentId;
        QuestionId = questionId;
        QuizAttemptId = quizAttemptId;
    }

    private QuestionAnswer()
    {
    }

    protected static Result<Validated> ValidateCommon(
        Guid studentId,
        Guid questionId,
        Guid quizAttemptId)
    {
        if (studentId == Guid.Empty)
        {
            return QuestionAnswerErrors.StudentIdRequired;
        }

        if (questionId == Guid.Empty)
        {
            return QuestionAnswerErrors.QuestionIdRequired;
        }

        if (quizAttemptId == Guid.Empty)
        {
            return QuestionAnswerErrors.QuizAttemptIdRequired;
        }

        return Result.Validated;
    }
}
