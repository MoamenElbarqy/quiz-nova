using System.Diagnostics.CodeAnalysis;

using QuizNova.Domain.Common;
using QuizNova.Domain.Common.Results;
using QuizNova.Domain.Entities.Courses.Enums;
using QuizNova.Domain.Entities.Courses.Events;
using QuizNova.Domain.Entities.Enrollments;
using QuizNova.Domain.Entities.Enrollments.Events;
using QuizNova.Domain.Entities.Users.Student;

namespace QuizNova.Domain.Entities.Courses;

public sealed class Course : Entity
{
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
        int maximumMarks)
        : base(id)
    {
        InstructorId = instructorId;
        Name = name;
        MinimumPassingMarks = minimumPassingMarks;
        MaximumMarks = maximumMarks;
        RemainingMarks = maximumMarks;
        Status = CourseStatus.Active;
    }

    public Guid? InstructorId { get; private set; }

    public CourseStatus Status { get; private set; }

    public required string Name { get; init; }

    public int EnrollmentsCount { get; private set; }

    public int MinimumPassingMarks { get; private set; }

    public int MaximumMarks { get; private set; }

    public int RemainingMarks { get; private set; }

    public static Result<Course> Create(
        Guid? instructorId,
        string name,
        int minimumPassingMarks,
        int maximumMarks)
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
            maximumMarks);
        course.AddDomainEvent(new CourseCreatedEvent(id));
        return course;
    }

    public Result<Enrollment> Enroll(Student student)
    {
        if (Status == CourseStatus.Completed)
        {
            return CourseErrors.CannotEnrollInCompletedCourse;
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

        EnrollmentsCount++;

        AddDomainEvent(new StudentEnrolledEvent(Id, student.Id));

        return enrollmentResult.Value;
    }

    public Result<Deleted> Disenroll(Student student)
    {
        if (Status == CourseStatus.Completed)
        {
            return CourseErrors.CannotDisenrollFromCompletedCourse;
        }

        if (EnrollmentsCount > 0)
        {
            EnrollmentsCount--;
        }

        AddDomainEvent(new StudentDisenrolledEvent(Id, student.Id));

        return Result.Deleted;
    }

    public Result<Updated> ConsumeMarks(int marks)
    {
        if (marks <= 0)
        {
            return CourseErrors.MarksInvalid;
        }

        if (marks > RemainingMarks)
        {
            return CourseErrors.InsufficientRemainingMarks(RemainingMarks, marks);
        }

        RemainingMarks -= marks;
        return Result.Updated;
    }

    public Result<Updated> ReleaseMarks(int marks)
    {
        if (marks <= 0)
        {
            return CourseErrors.MarksInvalid;
        }

        RemainingMarks += marks;

        if (RemainingMarks > MaximumMarks)
        {
            RemainingMarks = MaximumMarks;
        }

        return Result.Updated;
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
        return Result.Deleted;
    }

    public Result<Updated> MarkAsCompeleted()
    {
        Status = CourseStatus.Completed;
        return Result.Updated;
    }
}
