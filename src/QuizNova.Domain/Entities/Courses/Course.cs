using QuizNova.Domain.Common;
using QuizNova.Domain.Common.Results;
using QuizNova.Domain.Entities.Quizzes;
using QuizNova.Domain.Entities.Users.Instructors;

namespace QuizNova.Domain.Entities.Courses;

public sealed class Course : Entity
{
    private readonly List<Quiz> _quizzes;

    private Course()
    {
    }

    private Course(
        Guid id,
        Guid? instructorId,
        string name,
        int minimumPassingMarks,
        int maximumMarks,
        List<Quiz> quizzes)
        : base(id)
    {
        InstructorId = instructorId;
        Name = name;
        MinimumPassingMarks = minimumPassingMarks;
        MaximumMarks = maximumMarks;
        _quizzes = quizzes;
    }

    public Guid? InstructorId { get; private set; }

    public string Name { get; private set; } = string.Empty;

    public int MinimumPassingMarks { get; private set; }

    public int MaximumMarks { get; private set; }

    public Instructor? Instructor { get; }

    public IEnumerable<Quiz> Quizzes => _quizzes.AsReadOnly();

    public static Result<Course> Create(
        Guid id,
        Guid? instructorId,
        string name,
        int minimumPassingMarks,
        int maximumMarks,
        List<Quiz> quizzes)
    {
        if (instructorId.HasValue && instructorId.Value == Guid.Empty)
        {
            return CourseErrors.InstructorIdRequired;
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            return CourseErrors.NameRequired;
        }

        if (minimumPassingMarks <= 0)
        {
            return CourseErrors.MinimumPassingMarksInvalid;
        }

        if (maximumMarks <= 0)
        {
            return CourseErrors.MaximumMarksInvalid;
        }

        if (minimumPassingMarks > maximumMarks)
        {
            return CourseErrors.ScoringRangeInvalid;
        }

        return new Course(
            id,
            instructorId,
            name,
            minimumPassingMarks,
            maximumMarks,
            quizzes);
    }
}
