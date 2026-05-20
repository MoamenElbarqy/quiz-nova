using System.Diagnostics.CodeAnalysis;

using QuizNova.Domain.Common;
using QuizNova.Domain.Common.Results;
using QuizNova.Domain.Entities.Courses.Enums;
using QuizNova.Domain.Entities.Courses.Events;
using QuizNova.Domain.Entities.Enrollments;
using QuizNova.Domain.Entities.Quizzes;
using QuizNova.Domain.Entities.Users.Instructors;
using QuizNova.Domain.Entities.Users.Student;

namespace QuizNova.Domain.Entities.Courses;

public sealed class Course : Entity
{
    private readonly List<Quiz> _quizzes;
    private readonly List<Enrollment> _enrollments;

    [SetsRequiredMembers]
    private Course()
    {
    }

    [SetsRequiredMembers]
    private Course(
        Guid id,
        Guid? instructorId,
        string name,
        int minimumPassingMarks,
        int maximumMarks,
        List<Quiz> quizzes,
        List<Enrollment> enrollments)
        : base(id)
    {
        InstructorId = instructorId;
        Name = name;
        MinimumPassingMarks = minimumPassingMarks;
        MaximumMarks = maximumMarks;
        Status = CourseStatus.Active;
        _quizzes = quizzes;
        _enrollments = enrollments;
    }

    public Guid? InstructorId { get; private set; }

    public CourseStatus Status { get; private set; }

    public required string Name { get; init; }

    public int MinimumPassingMarks { get; private set; }

    public int MaximumMarks { get; private set; }

    // Remaining marks available to create quizzes in this course.
    public int RemainingMarks => MaximumMarks - Quizzes.Sum(q => q.Marks);

    public Instructor? Instructor { get; }

    public IEnumerable<Quiz> Quizzes => _quizzes.AsReadOnly();

    public IEnumerable<Enrollment> Enrollments => _enrollments.AsReadOnly();

    public static Result<Course> Create(
        Guid? instructorId,
        string name,
        int minimumPassingMarks,
        int maximumMarks,
        List<Quiz> quizzes,
        List<Enrollment> enrollments)
    {
        var id = Guid.NewGuid();

        if (instructorId.HasValue && instructorId.Value == Guid.Empty)
        {
            return CourseErrors.InstructorIdRequired;
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            return CourseErrors.NameRequired;
        }

        var trimmedName = name.Trim();
        if (trimmedName.Length < 3 || trimmedName.Length > 30)
        {
            return CourseErrors.NameInvalid;
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

        var course = new Course(
            id,
            instructorId,
            trimmedName,
            minimumPassingMarks,
            maximumMarks,
            quizzes,
            enrollments);
        course.AddDomainEvent(new CourseCreatedEvent(id));
        return course;
    }

    public Result<Enrollment> Enroll(Student student)
    {
        if (Status == CourseStatus.Completed)
        {
            return CourseErrors.CannotEnrollInCompletedCourse;
        }

        if (_enrollments.Any(sc => sc.StudentId == student.Id))
        {
            return CourseErrors.StudentAlreadyEnrolled(student.Id);
        }

        var enrollmentResult = Enrollment.Create(
            Guid.NewGuid(),
            student.Id,
            Id,
            DateTimeOffset.UtcNow);

        if (enrollmentResult.IsError)
        {
            return enrollmentResult.TopError;
        }

        _enrollments.Add(enrollmentResult.Value);
        AddDomainEvent(new StudentEnrolledEvent(Id, student.Id));

        return enrollmentResult.Value;
    }

    public Result<Course> UpdateInstructor(Guid? instructorId)
    {
        if (Status == CourseStatus.Completed)
        {
            return CourseErrors.CannotUpdateCompletedCourse;
        }

        if (instructorId.HasValue && instructorId.Value == Guid.Empty)
        {
            return CourseErrors.InstructorIdRequired;
        }

        InstructorId = instructorId;
        AddDomainEvent(new CourseUpdatedEvent(Id));

        return this;
    }

    public Result<Deleted> Delete()
    {
        AddDomainEvent(new CourseDeletedEvent(Id));
        return Result.Deleted;
    }

    public Result<Updated> MarkAsCompeleted()
    {
        Status = CourseStatus.Completed;
        AddDomainEvent(new CourseCompletedEvent(Id));
        return Result.Updated;
    }
}
