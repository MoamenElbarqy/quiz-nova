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

    public static readonly Error QuestionsRequired =
        Error.Validation("Quiz_Questions_Required", "At least 1 question is required to instantiate a quiz.");

    public static Error QuestionBelongsToDifferentQuiz(Guid questionId) =>
        Error.Validation(
            "Quiz_Question_BelongsToDifferentQuiz",
            $"Question with ID '{questionId}' belongs to a different quiz.");

    public static Error QuestionAlreadyExists(Guid questionId) =>
        Error.Validation(
            "Quiz_Question_AlreadyExists",
            $"Question with ID '{questionId}' already exists in the quiz.");

    public static readonly Error MinimumQuestionsReached =
        Error.Validation("Quiz_MinimumQuestionsReached", "A quiz must have at least 1 question.");

    public static readonly Error QuestionNotFound =
        Error.NotFound("Quiz_QuestionNotFound", "Question not found in the quiz.");

    public static readonly Error CannotUpdateStartedOrCompletedQuiz =
        Error.Validation(
            "Quiz_CannotUpdateStartedOrCompletedQuiz",
            "Cannot edit a quiz that has already started or completed.");

    public static readonly Error TitleTooShort =
        Error.Validation("Quiz_Title_TooShort", "Quiz title must be at least 3 characters long.");

    public static readonly Error TitleTooLong =
        Error.Validation("Quiz_Title_TooLong", "Quiz title cannot exceed 30 characters.");

    public static readonly Error CourseCompleted =
        Error.Validation("Quiz_CourseCompleted", "Cannot perform operations on a quiz belonging to a completed course.");

    public static Error CourseMismatch(Guid passedCourseId, Guid quizCourseId) =>
        Error.Validation(
            "Quiz_CourseMismatch",
            $"Course with ID '{passedCourseId}' does not match the quiz's course ID '{quizCourseId}'.");

    public static readonly Error ScheduleDurationTooShort =
        Error.Validation("Quiz_Schedule_DurationTooShort", "Quiz start and end time must be at least 10 minutes apart.");
}
