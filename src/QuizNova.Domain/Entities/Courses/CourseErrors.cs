using QuizNova.Domain.Common.Results;

namespace QuizNova.Domain.Entities.Courses;

public static class CourseErrors
{
    public static readonly Error InstructorIdRequired =
        Error.Validation("Course_InstructorId_Required", "Instructor ID is required.");

    public static readonly Error NameRequired =
        Error.Validation("Course_Name_Required", "Course name is required.");

    public static readonly Error NameInvalid =
        Error.Validation("Course_Name_Invalid", "Course name is invalid.");

    public static readonly Error MinimumPassingMarksInvalid =
        Error.Validation("Course_MinimumPassingMarks_Invalid", "Minimum passing marks cannot be negative.");

    public static readonly Error MaximumMarksInvalid =
        Error.Validation("Course_MaximumMarks_Invalid", "Maximum marks must be greater than zero.");

    public static readonly Error ScoringRangeInvalid =
        Error.Validation("Course_ScoringRange_Invalid", "Scoring configuration is inconsistent.");

    public static readonly Error CannotEnrollInCompletedCourse =
        Error.Validation("Course_CannotEnroll_Completed", "Cannot enroll in a completed course.");

    public static readonly Error CannotUpdateCompletedCourse =
        Error.Validation("Course_CannotUpdate_Completed", "Cannot update a completed course.");

    public static Error StudentAlreadyEnrolled(Guid studentId) =>
        Error.Conflict("Course_Student_Already_Enrolled", $"Student {studentId} is already enrolled in this course.");

    public static readonly Error MarksInvalid =
        Error.Validation("Course_Marks_Invalid", "Marks value is invalid.");

    public static Error InsufficientRemainingMarks(int remaining, int requested) =>
        Error.Validation("Course_Insufficient_Remaining_Marks",
            $"Insufficient remaining marks. Available: {remaining}, Requested: {requested}.");

    public static readonly Error CannotDisenrollFromCompletedCourse =
        Error.Validation("Course_CannotDisenroll_Completed", "Cannot disenroll from a completed course.");
}
