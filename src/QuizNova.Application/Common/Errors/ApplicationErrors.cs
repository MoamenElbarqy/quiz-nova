using QuizNova.Domain.Common.Results;

namespace QuizNova.Application.Common.Errors;

public static class ApplicationErrors
{
    public static readonly Error InvalidRefreshToken = Error.Unauthorized(
        code: "Auth.RefreshToken.Invalid",
        description: "Refresh token is invalid or has expired.");

    public static readonly Error ExpiredAccessTokenInvalid = Error.Conflict(
        code: "Auth.ExpiredAccessToken.Invalid",
        description: "Expired access token is not valid.");

    public static readonly Error UserIdClaimInvalid = Error.Conflict(
        code: "Auth.UserIdClaim.Invalid",
        description: "Invalid userId claim.");

    public static readonly Error UserNotFound = Error.NotFound(
        code: "Auth.User.NotFound",
        description: "User not found.");

    public static readonly Error TokenGenerationFailed = Error.Failure(
        code: "Auth.TokenGeneration.Failed",
        description: "Failed to generate new JWT token.");


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

    public static Error StudentNotFound(Guid studentId) =>
        Error.NotFound(
            code: "Student.NotFound",
            description: $"Student with ID '{studentId}' was not found.");

    public static Error AdminNotFound(Guid adminId) =>
        Error.NotFound(
            code: "Admin.NotFound",
            description: $"Admin with ID '{adminId}' was not found.");

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

    public static Error QuizNotFound(Guid quizId) =>
        Error.NotFound(
            code: "Quiz.NotFound",
            description: $"Quiz with ID '{quizId}' was not found.");

    public static Error StudentNotEnrolledInCourse(Guid studentId, Guid courseId) =>
        Error.Validation(
            code: "StudentCourse.Enrollment.NotFound",
            description: $"Student with ID '{studentId}' is not enrolled in course '{courseId}'.");

    public static Error QuizAttemptAlreadyExists(Guid studentId, Guid quizId) =>
        Error.Conflict(
            code: "QuizAttempt.AlreadyExists",
            description: $"Student '{studentId}' already has an attempt for quiz '{quizId}'.");

    public static Error QuizQuestionIdAlreadyExists(Guid questionId) =>
        Error.Conflict(
            code: "Quiz.Question.Id.AlreadyExists",
            description: $"Question with ID '{questionId}' already exists.");

    public static Error QuizCorrectChoiceNotFound(Guid questionId, Guid correctChoiceId) =>
        Error.Validation(
            code: "Quiz.Question.CorrectChoice.NotFound",
            description:
            $"Correct choice with ID '{correctChoiceId}' was not found for question with ID '{questionId}'.");

    public static Error QuizChoiceIdsMustBeUnique(Guid questionId) =>
        Error.Validation(
            code: "Quiz.Question.ChoiceIds.NotUnique",
            description: $"Choice IDs must be unique for question with ID '{questionId}'.");

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

    public static Error UserIdAlreadyExists(Guid userId) =>
        Error.Conflict(
            code: "User.Id.AlreadyExists",
            description: $"User with ID '{userId}' already exists.");

    public static Error UserEmailAlreadyExists(string email) =>
        Error.Conflict(
            code: "User.Email.AlreadyExists",
            description: $"User with email '{email}' already exists.");

    public static Error UserPhoneNumberAlreadyExists(string phoneNumber) =>
        Error.Conflict(
            code: "User.PhoneNumber.AlreadyExists",
            description: $"User with phone number '{phoneNumber}' already exists.");

    public static Error UserRoleInvalid(string role) =>
        Error.Validation(
            code: "User.Role.Invalid",
            description: $"Role '{role}' is not a valid role.");

    public static Error CreateInstructorRoleInvalid(string role) =>
        Error.Validation(
            code: "Instructor.Role.Invalid",
            description: $"Role '{role}' is invalid for instructor creation.");

    public static Error CreateStudentRoleInvalid(string role) =>
        Error.Validation(
            code: "Student.Role.Invalid",
            description: $"Role '{role}' is invalid for student creation.");

    public static Error CreateAdminRoleInvalid(string role) =>
        Error.Validation(
            code: "Admin.Role.Invalid",
            description: $"Role '{role}' is invalid for admin creation.");
}
