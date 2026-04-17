using QuizNova.Domain.Common;
using QuizNova.Domain.Common.Results;
using QuizNova.Domain.Entities.Quizzes.Questions.Base;
using QuizNova.Domain.Entities.Users.Student;

namespace QuizNova.Domain.Entities.QuizAttempts.Answers.Base;

public class QuestionAnswer : Entity
{
    public Guid StudentId { get; private set; }

    public Guid QuestionId { get; private set; }

    public Guid QuizAttemptId { get; private set; }

    public QuizAttempt? QuizAttempt { get; private set; }

    public Student? Student { get; private set; }

    public Question? Question { get; private set; }

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
            return StudentAnswerErrors.StudentIdRequired;
        }

        if (questionId == Guid.Empty)
        {
            return StudentAnswerErrors.QuestionIdRequired;
        }

        if (quizAttemptId == Guid.Empty)
        {
            return StudentAnswerErrors.QuizAttemptIdRequired;
        }

        return Result.Validated;
    }
}
