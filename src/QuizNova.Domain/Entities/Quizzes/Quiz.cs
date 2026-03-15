using QuizNova.Domain.Common;
using QuizNova.Domain.Common.Results;
using QuizNova.Domain.Entities.Courses;
using QuizNova.Domain.Entities.Quizzes.Questions.Base;
using QuizNova.Domain.Entities.Users;

namespace QuizNova.Domain.Entities.Quizzes;

public class Quiz : AuditableEntity
{
    private readonly List<Question> _questions;

    private Quiz(
        Guid id,
        Guid courseId,
        Guid instructorId,
        string title,
        DateTimeOffset startsAtUtc,
        DateTimeOffset endsAtUtc,
        int marks,
        List<Question> questions)
        : base(id)
    {
        CourseId = courseId;
        InstructorId = instructorId;
        Title = title;
        StartsAtUtc = startsAtUtc;
        EndsAtUtc = endsAtUtc;
        _questions = questions;
    }

    public Guid CourseId { get; private set; }

    public Guid InstructorId { get; private set; }

    public string Title { get; private set; } = string.Empty;

    public DateTimeOffset StartsAtUtc { get; private set; }

    public DateTimeOffset EndsAtUtc { get; private set; }

    public int Marks => Questions.Sum(q => q.Marks);

    public IEnumerable<Question> Questions => _questions.AsReadOnly();

    public Course? Course { get; private set; }

    public Instructor? Instructor { get; private set; }

    public static Result<Quiz> Create(
        Guid id,
        Guid courseId,
        Guid instructorId,
        string title,
        DateTimeOffset startsAtUtc,
        DateTimeOffset endsAtUtc,
        int marks,
        List<Question> questions)
    {
        if (courseId == Guid.Empty)
        {
            return QuizErrors.CourseIdRequired;
        }

        if (instructorId == Guid.Empty)
        {
            return QuizErrors.InstructorIdRequired;
        }

        if (string.IsNullOrWhiteSpace(title))
        {
            return QuizErrors.TitleRequired;
        }

        if (startsAtUtc >= endsAtUtc)
        {
            return QuizErrors.ScheduleInvalid;
        }

        if (marks <= 0)
        {
            return QuizErrors.MarksInvalid;
        }

        return new Quiz(id, courseId, instructorId, title, startsAtUtc, endsAtUtc, marks, questions);
    }
}
