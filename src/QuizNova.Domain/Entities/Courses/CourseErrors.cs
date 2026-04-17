using QuizNova.Domain.Common.Results;

namespace QuizNova.Domain.Entities.Courses;

public static class CourseErrors
{
    public static readonly Error InstructorIdRequired =
        Error.Validation("Course_InstructorId_Required", "Instructor ID is required.");

    public static readonly Error NameRequired =
        Error.Validation("Course_Name_Required", "Course name is required.");

    public static readonly Error MinimumPassingMarksInvalid =
        Error.Validation("Course_MinimumPassingMarks_Invalid", "Minimum passing marks cannot be negative.");

    public static readonly Error MaximumMarksInvalid =
        Error.Validation("Course_MaximumMarks_Invalid", "Maximum marks must be greater than zero.");

    public static readonly Error ScoringRangeInvalid =
        Error.Validation("Course_ScoringRange_Invalid", "Scoring configuration is inconsistent.");
}
