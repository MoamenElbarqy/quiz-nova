using QuizNova.Domain.Common.Results;

namespace QuizNova.Application.Common.Errors;

public static class ApplicationErrors
{
    public static Error InvalidRefreshToken =>
    Error.Validation(
        "RefreshToken.Expiry.Invalid",
        "Expiry must be in the future.");

    public static readonly Error ExpiredAccessTokenInvalid = Error.Conflict(
         code: "Auth.ExpiredAccessToken.Invalid",
         description: "Expired access token is not valid.");

    public static readonly Error UserIdClaimInvalid = Error.Conflict(
        code: "Auth.UserIdClaim.Invalid",
        description: "Invalid userId claim.");

    public static readonly Error RefreshTokenExpired = Error.Conflict(
        code: "Auth.RefreshToken.Expired",
        description: "Refresh token is invalid or has expired.");

    public static readonly Error UserNotFound = Error.NotFound(
        code: "Auth.User.NotFound",
        description: "User not found.");

    public static readonly Error TokenGenerationFailed = Error.Failure(
        code: "Auth.TokenGeneration.Failed",
        description: "Failed to generate new JWT token.");

    public static Error CollegeNotFound(Guid collegeId) =>
        Error.NotFound(
            code: "Student_CollegeNotFound",
            description: $"College with ID '{collegeId}' was not found.");

    public static Error InstructorNotFound(Guid instructorId) =>
        Error.NotFound(
            code: "Instructor_NotFound",
            description: $"Instructor with ID '{instructorId}' was not found.");

    public static Error NoCoursesForInstructor(Guid instructorId) =>
        Error.NotFound(
            code: "Courses_NoCoursesForInstructor",
            description: $"No courses found for instructor with ID '{instructorId}'.");

    public static Error QuizAttemptStudentNotFound(Guid studentId) =>
        Error.NotFound(
            code: "QuizAttempt.Student.NotFound",
            description: $"Student with ID '{studentId}' was not found.");

    public static Error QuizIdAlreadyExists(Guid quizId) =>
        Error.Conflict(
            code: "Quiz.Id.AlreadyExists",
            description: $"Quiz with ID '{quizId}' already exists.");

    public static Error QuizCourseNotFound(Guid courseId) =>
        Error.NotFound(
            code: "Quiz.Course.NotFound",
            description: $"Course with ID '{courseId}' was not found.");

    public static Error QuizInstructorNotFound(Guid instructorId) =>
        Error.NotFound(
            code: "Quiz.Instructor.NotFound",
            description: $"Instructor with ID '{instructorId}' was not found.");

    public static Error QuizInstructorIsNotAssignedToCourse(Guid instructorId, Guid courseId) =>
        Error.Validation(
            code: "Quiz.InstructorCourse.Invalid",
            description: $"Instructor with ID '{instructorId}' is not assigned to course with ID '{courseId}'.");

    public static Error QuizQuestionIdAlreadyExists(Guid questionId) =>
        Error.Conflict(
            code: "Quiz.Question.Id.AlreadyExists",
            description: $"Question with ID '{questionId}' already exists.");

    public static Error QuizCorrectChoiceNotFound(Guid questionId, Guid correctChoiceId) =>
        Error.Validation(
            code: "Quiz.Question.CorrectChoice.NotFound",
            description: $"Correct choice with ID '{correctChoiceId}' was not found for question with ID '{questionId}'.");

    public static Error QuizChoiceIdsMustBeUnique(Guid questionId) =>
        Error.Validation(
            code: "Quiz.Question.ChoiceIds.NotUnique",
            description: $"Choice IDs must be unique for question with ID '{questionId}'.");

    public static Error QuizChoiceIdAlreadyExists(Guid choiceId) =>
        Error.Conflict(
            code: "Quiz.Question.Choice.Id.AlreadyExists",
            description: $"Choice with ID '{choiceId}' already exists.");

    public static Error QuizChoiceBelongsToDifferentQuestion(Guid choiceId, Guid questionId) =>
        Error.Validation(
            code: "Quiz.Question.Choice.QuestionId.Invalid",
            description: $"Choice with ID '{choiceId}' belongs to a different question than '{questionId}'.");

    public static Error QuizDuplicateQuestionIdsInRequest() =>
        Error.Validation(
            code: "Quiz.Request.DuplicateQuestionIds",
            description: "Duplicate question IDs found in the request.");

    public static Error QuizSomeIdAlreadyExists(Guid id) =>
        Error.Conflict(
            code: "Quiz.Request.Id.AlreadyExists",
            description: $"An entity with ID '{id}' already exists.");
}