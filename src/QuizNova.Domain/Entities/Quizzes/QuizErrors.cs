using QuizNova.Domain.Common.Results;

namespace QuizNova.Domain.Entities.Quizzes;

public static class QuizErrors
{
    public static readonly Error CourseIdRequired =
        Error.Validation("Quiz_CourseId_Required", "Course ID is required.");

    public static readonly Error InstructorIdRequired =
        Error.Validation("Quiz_InstructorId_Required", "Instructor ID is required.");

    public static readonly Error TitleRequired =
        Error.Validation("Quiz_Title_Required", "Quiz title is required.");

    public static readonly Error ScheduleInvalid =
        Error.Validation("Quiz_Schedule_Invalid", "Quiz start time must be earlier than end time.");

    public static readonly Error MarksInvalid =
        Error.Validation("Quiz_Marks_Invalid", "Marks must be greater than zero.");
}
