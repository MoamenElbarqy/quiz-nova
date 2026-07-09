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
        _studentAnswers = [];
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

    public GradingState GradingState => _studentAnswers.All(a => a is not ManuallyGradedAnswers { IsGraded: false })
        ? GradingState.FullyGraded
        : GradingState.AwaitingGrading;

    public int Score => StudentAnswers.Sum(answer => answer switch
    {
        AutoGradedAnswer autoGradedAnswer when autoGradedAnswer.Question is not null && autoGradedAnswer.IsCorrect =>
            autoGradedAnswer.Question.Marks,
        ManuallyGradedAnswers manuallyGradedAnswer => manuallyGradedAnswer.Score ?? 0,
        _ => 0
    });

    public IEnumerable<QuestionAnswer> StudentAnswers => _studentAnswers.AsReadOnly();

    public static Result<QuizAttempt> Start(
        Guid id,
        Guid studentId,
        Guid quizId,
        DateTime startedAt)
    {
        if (id == Guid.Empty)
        {
            return QuizAttemptErrors.AttemptIdRequired;
        }

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

        var quizAttempt = new QuizAttempt(
            id,
            studentId,
            quizId,
            startedAt,
            default,
            QuizAttemptStatus.InProgress,
            []);

        return quizAttempt;
    }

    public Result<Validated> SubmitAnswer(QuestionAnswer answer)
    {
        if (Status != QuizAttemptStatus.InProgress)
        {
            return QuizAttemptErrors.AttemptAlreadyCompleted;
        }

        if (answer is null)
        {
            return QuizAttemptErrors.QuestionAnswerRequired;
        }

        if (Quiz is not null && Quiz.Questions.All(q => q.Id != answer.QuestionId))
        {
            return QuizAttemptErrors.QuestionNotFoundInQuiz(answer.QuestionId, QuizId);
        }

        _studentAnswers.RemoveAll(a => a.QuestionId == answer.QuestionId);
        _studentAnswers.Add(answer);

        return Result.Validated;
    }

    public Result<Completed> Complete(DateTime submittedAt, DateTime quizEndsAtUtc)
    {
        if (Status != QuizAttemptStatus.InProgress)
        {
            return QuizAttemptErrors.AttemptAlreadyCompleted;
        }

        if (submittedAt == default)
        {
            return QuizAttemptErrors.SubmittedAtRequired;
        }

        if (submittedAt < StartedAt)
        {
            return QuizAttemptErrors.SubmittedAtInvalid;
        }

        if (submittedAt > quizEndsAtUtc)
        {
            return QuizAttemptErrors.SubmittedAtAfterQuizEnd(quizEndsAtUtc);
        }

        SubmittedAt = submittedAt;
        Status = QuizAttemptStatus.Completed;

        return Result.Completed;
    }
}
