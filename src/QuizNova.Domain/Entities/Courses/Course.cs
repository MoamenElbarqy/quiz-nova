using QuizNova.Domain.Common;
using QuizNova.Domain.Common.Results;
using QuizNova.Domain.Entities.Quizzes;
using QuizNova.Domain.Entities.Users;

namespace QuizNova.Domain.Entities.Courses;

public sealed class Course : AuditableEntity
{

    private readonly List<Quiz> _quizzes;

    private Course()
    {
        _quizzes = new List<Quiz>();
    }

    private Course(
        Guid id,
        Guid instructorId,
        string name,
        int minimumPassingMarks,
        int maximumMarks,
        bool isGraceMarksActivated,
        int? maxGraceMarks,
        List<Quiz> quizzes)
        : base(id)
    {
        InstructorId = instructorId;
        Name = name;
        MinimumPassingMarks = minimumPassingMarks;
        MaximumMarks = maximumMarks;
        IsGraceMarksActivated = isGraceMarksActivated;
        MaxGraceMarks = maxGraceMarks;
        _quizzes = quizzes;
    }

    public Guid InstructorId { get; private set; }

    public string Name { get; private set; } = string.Empty;

    public int TotalMarks { get; private set; }

    public int MinimumPassingMarks { get; private set; }

    public int MaximumMarks { get; private set; }

    public bool IsGraceMarksActivated { get; private set; }

    public int? MaxGraceMarks { get; private set; }

    public Instructor? Instructor { get; private set; }

    public IEnumerable<Quiz> Quizzes => _quizzes.AsReadOnly();

    public static Result<Course> Create(
        Guid id,
        Guid departmentId,
        Guid instructorId,
        string name,
        int minimumPassingMarks,
        int maximumMarks,
        bool isGraceMarksActivated,
        int? maxGraceMarks,
        List<Quiz> quizzes)
    {
        if (departmentId == Guid.Empty)
        {
            return CourseErrors.DepartmentIdRequired;
        }

        if (instructorId == Guid.Empty)
        {
            return CourseErrors.InstructorIdRequired;
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            return CourseErrors.NameRequired;
        }

        if (minimumPassingMarks < 0)
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

        if (isGraceMarksActivated && maxGraceMarks == null)
        {
            return CourseErrors.MaxGraceMarksRequired;
        }

        if (isGraceMarksActivated && maxGraceMarks < 0)
        {
            return CourseErrors.MaxGraceMarksInvalid;
        }

        return new Course(
            id,
            instructorId,
            name,
            minimumPassingMarks,
            maximumMarks,
            isGraceMarksActivated,
            maxGraceMarks,
            quizzes);
    }
}
