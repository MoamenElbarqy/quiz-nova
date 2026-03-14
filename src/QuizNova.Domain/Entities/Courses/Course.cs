using QuizNova.Domain.Common;
using QuizNova.Domain.Common.Results;
using QuizNova.Domain.Entities.StudentCourses;
using QuizNova.Domain.Entities.Colleges;
using QuizNova.Domain.Entities.Departments;
using QuizNova.Domain.Entities.Quizzes;
using QuizNova.Domain.Entities.Users;

namespace QuizNova.Domain.Entities.Courses;

public class Course : AuditableEntity
{
    public Guid CollegeId { get; private set; }

    public Guid InstructorId { get; private set; }

    public string Name { get; private set; } = string.Empty;

    public int TotalPoints { get; private set; }

    public int MinimumPassingPoints { get; private set; }

    public int MaximumMarks { get; private set; }

    public bool IsGraceMarksActivated { get; private set; }
    public int? MaxGraceMarks { get; private set; }

    public College? College { get; private set; }

    public List<Department>? Departments = [];

    public Instructor? Instructor { get; private set; }

    public List<StudentCourse> StudentEnrollments { get; private set; } = [];

    public List<Student> Students { get; private set; } = [];

    public List<Quiz> Quizzes { get; private set; } = [];

    private Course()
    {
    }

    private Course(
        Guid id,
        Guid collegeId,
        Guid instructorId,
        string name,
        int minimumPassingMarks,
        int maximumMarks,
        bool isGraceMarksActivated,
        int? maxGraceMarks)
        : base(id)
    {
        CollegeId = collegeId;
        InstructorId = instructorId;
        Name = name;
        MinimumPassingPoints = minimumPassingMarks;
        MaximumMarks = maximumMarks;
        IsGraceMarksActivated = isGraceMarksActivated;
        MaxGraceMarks = maxGraceMarks;
    }

    public static Result<Course> Create(
        Guid id,
        Guid collegeId,
        Guid departmentId,
        Guid instructorId,
        string name,
        int minimumPassingMarks,
        int maximumMarks,
        bool isGraceMarksActivated,
        int? maxGraceMarks)
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
            return CourseErrors.MinimumPassingPointsInvalid;
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
            maxGraceMarks);
    }
}
