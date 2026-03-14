using QuizNova.Domain.Common;
using QuizNova.Domain.Common.Results;
using QuizNova.Domain.Entities.Users;
using QuizNova.Domain.Entities.Quizzes;
using QuizNova.Domain.Entities.QuizAttempts.Answers.Base;

namespace QuizNova.Domain.Entities.QuizAttempts;

public class QuizAttempt : AuditableEntity
{
    public Guid StudentId { get; private set; }
    public Guid QuizId { get; private set; }

    public Student? Student { get; private set; }
    public Quiz? Quiz { get; private set; }

    public List<StudentAnswer> StudentAnswers { get; private set; } = [];

    public DateTime StartedAt { get; private set; }
    public DateTime? SubmittedAt { get; private set; }

    private QuizAttempt()
    {
    }

    private QuizAttempt(Guid id, Guid studentId, Guid quizId, DateTime startedAt, DateTime? submittedAt)
        : base(id)
    {
        StudentId = studentId;
        QuizId = quizId;
        StartedAt = startedAt;
        SubmittedAt = submittedAt;
    }

    public static Result<QuizAttempt> Create(
        Guid id,
        Guid studentId,
        Guid quizId,
        DateTime startedAt,
        DateTime? submittedAt)
    {
        if (studentId == Guid.Empty)
        {
            return QuizAttemptErrors.StudentIdRequired;
        }

        if (quizId == Guid.Empty)
        {
            return QuizAttemptErrors.QuizIdRequired;
        }

        if (startedAt == default)
        {
            return QuizAttemptErrors.StartedAtRequired;
        }

        if (submittedAt.HasValue && submittedAt.Value < startedAt)
        {
            return QuizAttemptErrors.SubmittedAtInvalid;
        }

        return new QuizAttempt(id, studentId, quizId, startedAt, submittedAt);
    }
}