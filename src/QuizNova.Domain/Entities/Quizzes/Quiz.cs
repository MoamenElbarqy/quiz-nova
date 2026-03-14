using QuizNova.Domain.Common;
using QuizNova.Domain.Common.Results;
using QuizNova.Domain.Entities..Courses;
using QuizNova.Domain.Entities.Users;

namespace QuizNova.Domain.Entities.Quizzes;

public class Quiz : AuditableEntity
{
    public Guid CourseId { get; private set; }

    public Guid InstructorId { get; private set; }

    public string Title { get; private set; } = string.Empty;

    public DateTimeOffset StartsAtUtc { get; private set; }

    public DateTimeOffset EndsAtUtc { get; private set; }

    public int Points { get; private set; }

    public Course? Course { get; private set; }

    public Instructor? Instructor { get; private set; }

    private Quiz()
    {
    }

    private Quiz(
        Guid id,
        Guid courseId,
        Guid instructorId,
        string title,
        DateTimeOffset startsAtUtc,
        DateTimeOffset endsAtUtc,
        int points)
        : base(id)
    {
        CourseId = courseId;
        InstructorId = instructorId;
        Title = title;
        StartsAtUtc = startsAtUtc;
        EndsAtUtc = endsAtUtc;
        Points = points;
    }

    public static Result<Quiz> Create(
        Guid id,
        Guid courseId,
        Guid instructorId,
        string title,
        DateTimeOffset startsAtUtc,
        DateTimeOffset endsAtUtc,
        int points)
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

        if (points <= 0)
        {
            return QuizErrors.PointsInvalid;
        }

        return new Quiz(id, courseId, instructorId, title, startsAtUtc, endsAtUtc, points);
    }
}
