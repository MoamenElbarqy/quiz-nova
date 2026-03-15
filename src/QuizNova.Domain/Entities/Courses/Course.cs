using QuizNova.Domain.Common;
using QuizNova.Domain.Common.Results;
using QuizNova.Domain.Entities.Colleges;
using QuizNova.Domain.Entities.Departments;
using QuizNova.Domain.Entities.Quizzes;
using QuizNova.Domain.Entities.StudentCourses;
using QuizNova.Domain.Entities.Users;

namespace QuizNova.Domain.Entities.Courses;

public sealed class Course : AuditableEntity
{
    private readonly List<Department> _departments;

    private readonly List<StudentCourse> _studentEnrollments;

    private readonly List<Student> _students;

    private readonly List<Quiz> _quizzes;

    private Course(
        Guid id,
        Guid collegeId,
        Guid instructorId,
        string name,
        int minimumPassingMarks,
        int maximumMarks,
        bool isGraceMarksActivated,
        int? maxGraceMarks,
        List<Department> departments,
        List<StudentCourse> studentEnrollments,
        List<Student> students,
        List<Quiz> quizzes)
        : base(id)
    {
        CollegeId = collegeId;
        InstructorId = instructorId;
        Name = name;
        MinimumPassingMarks = minimumPassingMarks;
        MaximumMarks = maximumMarks;
        IsGraceMarksActivated = isGraceMarksActivated;
        MaxGraceMarks = maxGraceMarks;
        _departments = departments;
        _studentEnrollments = studentEnrollments;
        _students = students;
        _quizzes = quizzes;
    }

    public Guid CollegeId { get; private set; }

    public Guid InstructorId { get; private set; }

    public string Name { get; private set; } = string.Empty;

    public int TotalMarks { get; private set; }

    public int MinimumPassingMarks { get; private set; }

    public int MaximumMarks { get; private set; }

    public bool IsGraceMarksActivated { get; private set; }

    public int? MaxGraceMarks { get; private set; }

    public College? College { get; private set; }

    public Instructor? Instructor { get; private set; }

    public IEnumerable<Department> Departments => _departments.AsReadOnly();

    public IEnumerable<StudentCourse> StudentEnrollments => _studentEnrollments.AsReadOnly();

    public IEnumerable<Student> Students => _students.AsReadOnly();

    public IEnumerable<Quiz> Quizzes => _quizzes.AsReadOnly();

    public static Result<Course> Create(
        Guid id,
        Guid collegeId,
        Guid departmentId,
        Guid instructorId,
        string name,
        int minimumPassingMarks,
        int maximumMarks,
        bool isGraceMarksActivated,
        int? maxGraceMarks,
        List<Department> departments,
        List<StudentCourse> studentEnrollments,
        List<Student> students,
        List<Quiz> quizzes)
    {
        if (collegeId == Guid.Empty)
        {
            return CourseErrors.CollegeIdRequired;
        }

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
            collegeId,
            instructorId,
            name,
            minimumPassingMarks,
            maximumMarks,
            isGraceMarksActivated,
            maxGraceMarks,
            departments,
            studentEnrollments,
            students,
            quizzes);
    }
}
