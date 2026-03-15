using QuizNova.Domain.Common;
using QuizNova.Domain.Common.Results;
using QuizNova.Domain.Entities.QuizAttempts.Answers.Base;
using QuizNova.Domain.Entities.Quizzes;
using QuizNova.Domain.Entities.Users;

namespace QuizNova.Domain.Entities.QuizAttempts;

public class QuizAttempt : AuditableEntity
{
    private readonly List<StudentAnswer> _studentAnswers;

    private QuizAttempt(
        Guid id,
        Guid studentId,
        Guid quizId,
        DateTime startedAt,
        DateTime? submittedAt,
        List<StudentAnswer> studentAnswers)
        : base(id)
    {
        StudentId = studentId;
        QuizId = quizId;
        StartedAt = startedAt;
        SubmittedAt = submittedAt;
        _studentAnswers = studentAnswers;
    }

    public Guid StudentId { get; private set; }

    public Guid QuizId { get; private set; }

    public DateTime StartedAt { get; private set; }

    public DateTime? SubmittedAt { get; private set; }

    public Student? Student { get; private set; }

    public Quiz? Quiz { get; private set; }

    public IEnumerable<StudentAnswer> StudentAnswers => _studentAnswers.AsReadOnly();

    public static Result<QuizAttempt> Create(
        Guid id,
        Guid studentId,
        Guid quizId,
        DateTime startedAt,
        DateTime? submittedAt,
        List<StudentAnswer> studentAnswers)
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

        return new QuizAttempt(id, studentId, quizId, startedAt, submittedAt, studentAnswers);
    }
}
