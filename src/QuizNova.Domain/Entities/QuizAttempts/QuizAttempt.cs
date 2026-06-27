using QuizNova.Domain.Common;
using QuizNova.Domain.Common.Results;
using QuizNova.Domain.Entities.QuizAttempts.Answers.AutoGradedAnswers;
using QuizNova.Domain.Entities.QuizAttempts.Answers.Base;
using QuizNova.Domain.Entities.QuizAttempts.Answers.ManuallyGradedAnswers;
using QuizNova.Domain.Entities.QuizAttempts.Enums;
using QuizNova.Domain.Entities.Quizzes;
using QuizNova.Domain.Entities.Users.Student;

namespace QuizNova.Domain.Entities.QuizAttempts;

public class QuizAttempt : Entity
{
    private readonly List<QuestionAnswer> _studentAnswers;

    private QuizAttempt()
    {
    }

    private QuizAttempt(
        Guid id,
        Guid studentId,
        Guid quizId,
        DateTime startedAt,
        DateTime submittedAt,
        QuizAttemptStatus status,
        List<QuestionAnswer> studentAnswers)
        : base(id)
    {
        StudentId = studentId;
        QuizId = quizId;
        StartedAt = startedAt;
        SubmittedAt = submittedAt;
        Status = status;
        _studentAnswers = studentAnswers;
    }

    public Guid StudentId { get; private set; }

    public Guid QuizId { get; private set; }

    public DateTime StartedAt { get; private set; }

    public DateTime SubmittedAt { get; private set; }

    public QuizAttemptStatus Status { get; private set; }

    public Student? Student { get; init; }

    public Quiz? Quiz { get; init; }

    public int Score => StudentAnswers.Sum(answer => answer switch
    {
        AutoGradedAnswer autoGradedAnswer when autoGradedAnswer.Question is not null && autoGradedAnswer.IsCorrect =>
            autoGradedAnswer.Question.Marks,
        ManuallyGradedAnswers manuallyGradedAnswer => manuallyGradedAnswer.Score ?? 0,
        _ => 0
    });

    public IEnumerable<QuestionAnswer> StudentAnswers => _studentAnswers.AsReadOnly();

    public static Result<QuizAttempt> Create(
        Guid id,
        Guid studentId,
        Guid quizId,
        DateTime startedAt,
        DateTime submittedAt,
        List<QuestionAnswer> studentAnswers)
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

        if (submittedAt < startedAt)
        {
            return QuizAttemptErrors.SubmittedAtInvalid;
        }

        var status = studentAnswers.Any(a => a is ManuallyGradedAnswers { IsGraded: false })
            ? QuizAttemptStatus.Pending
            : QuizAttemptStatus.Completed;

        var quizAttempt = new QuizAttempt(id, studentId, quizId, startedAt, submittedAt, status, studentAnswers);
        return quizAttempt;
    }
}
