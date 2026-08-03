using QuizNova.Domain.Common;
using QuizNova.Domain.Common.Results;
using QuizNova.Domain.Entities.QuizAttempts.Answers.AutoGradedAnswers;
using QuizNova.Domain.Entities.QuizAttempts.Answers.AutoGradedAnswers.McqAnswer;
using QuizNova.Domain.Entities.QuizAttempts.Answers.AutoGradedAnswers.TrueFalseAnswer;
using QuizNova.Domain.Entities.QuizAttempts.Answers.Base;
using QuizNova.Domain.Entities.QuizAttempts.Answers.ManuallyGradedAnswers;
using QuizNova.Domain.Entities.QuizAttempts.Answers.ManuallyGradedAnswers.EssayAnswer;
using QuizNova.Domain.Entities.QuizAttempts.Enums;
using QuizNova.Domain.Entities.Users.Student;

namespace QuizNova.Domain.Entities.QuizAttempts;

public class QuizAttempt : Entity
{
    private List<QuestionAnswer> _studentAnswers = [];

    private QuizAttempt()
    {
    }

    private QuizAttempt(
        Guid id,
        Guid studentId,
        Guid quizId,
        DateTime startedAt,
        DateTime? submittedAt,
        QuizAttemptStatus status,
        DateTimeOffset quizEndsAtUtc,
        List<QuestionAnswer> studentAnswers)
        : base(id)
    {
        StudentId = studentId;
        QuizId = quizId;
        StartedAt = startedAt;
        SubmittedAt = submittedAt;
        Status = status;
        QuizEndsAtUtc = quizEndsAtUtc;
        _studentAnswers = studentAnswers;
    }

    public Guid StudentId { get; private set; }

    public Guid QuizId { get; private set; }

    public DateTime StartedAt { get; private set; }

    public DateTime? SubmittedAt { get; private set; }

    public QuizAttemptStatus Status { get; private set; }

    public DateTimeOffset QuizEndsAtUtc { get; private set; }

    public Student? Student { get; init; }

    public GradingState GradingState => _studentAnswers.All(a => a is not ManuallyGradedAnswers { IsGraded: false })
        ? GradingState.FullyGraded
        : GradingState.AwaitingGrading;

    public int Score => StudentAnswers.Sum(answer => answer switch
    {
        AutoGradedAnswer autoGraded when autoGraded.IsCorrect => autoGraded.Marks,
        ManuallyGradedAnswers manuallyGraded => manuallyGraded.Score ?? 0,
        _ => 0
    });

    public IEnumerable<QuestionAnswer> StudentAnswers
    {
        get => _studentAnswers.AsReadOnly();
        private set => _studentAnswers = [.. value];
    }

    public static Result<QuizAttempt> Start(
        Guid id,
        Guid studentId,
        Guid quizId,
        DateTime startedAt,
        DateTimeOffset quizEndsAtUtc)
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
            null,
            QuizAttemptStatus.InProgress,
            quizEndsAtUtc,
            []);

        return quizAttempt;
    }

    public Result<Validated> SubmitAnswer(QuestionAnswer answer)
    {
        if (Status != QuizAttemptStatus.InProgress)
        {
            return QuizAttemptErrors.AttemptAlreadyCompleted;
        }

        var existingAnswer = _studentAnswers.FirstOrDefault(a => a.QuestionId == answer.QuestionId);
        if (existingAnswer is not null)
        {
            if (existingAnswer is McqAnswer existingMcq &&
                answer is McqAnswer newMcq)
            {
                existingMcq.Update(newMcq.SelectedChoiceId, newMcq.IsCorrect);
            }
            else if (existingAnswer is TfAnswer existingTf &&
                     answer is TfAnswer newTf)
            {
                existingTf.Update(newTf.StudentChoice, newTf.IsCorrect);
            }
            else if (existingAnswer is EssayAnswer existingEssay &&
                     answer is EssayAnswer newEssay)
            {
                existingEssay.Update(newEssay.StudentResponse);
            }
            else
            {
                _studentAnswers.Remove(existingAnswer);
                _studentAnswers.Add(answer);
            }
        }
        else
        {
            _studentAnswers.Add(answer);
        }

        return Result.Validated;
    }

    public Result<Completed> Complete(DateTime submittedAt)
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

        if (submittedAt > QuizEndsAtUtc)
        {
            return QuizAttemptErrors.SubmittedAtAfterQuizEnd(QuizEndsAtUtc);
        }

        SubmittedAt = submittedAt;
        Status = QuizAttemptStatus.Completed;

        return Result.Completed;
    }

}
