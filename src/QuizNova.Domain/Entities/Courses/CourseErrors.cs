using QuizNova.Domain.Common.Results;

namespace QuizNova.Domain.Entities.Courses;

public static class CourseErrors
{
    public static readonly Error CollegeIdRequired =
        Error.Validation("Course_CollegeId_Required", "College ID is required.");

    public static readonly Error DepartmentIdRequired =
        Error.Validation("Course_DepartmentId_Required", "Department ID is required.");

    public static readonly Error InstructorIdRequired =
        Error.Validation("Course_InstructorId_Required", "Instructor ID is required.");

    public static readonly Error NameRequired =
        Error.Validation("Course_Name_Required", "Course name is required.");

    public static readonly Error TotalPointsInvalid =
        Error.Validation("Course_TotalPoints_Invalid", "Total points must be greater than zero.");

    public static readonly Error MinimumPassingPointsInvalid =
        Error.Validation("Course_MinimumPassingPoints_Invalid", "Minimum passing points cannot be negative.");

    public static readonly Error MaximumMarksInvalid =
        Error.Validation("Course_MaximumMarks_Invalid", "Maximum marks must be greater than zero.");

    public static readonly Error ScoringRangeInvalid =
        Error.Validation("Course_ScoringRange_Invalid", "Scoring configuration is inconsistent.");

    public static readonly Error MaxGraceMarksInvalid =
        Error.Validation("Course_MaxGraceMarks_Invalid", "Maximum grace marks must be greater than or equal to zero.");

    public static readonly Error MaxGraceMarksRequired =
        Error.Validation("Course_MaxGraceMarks_Required", "Maximum grace marks is required.");
}
