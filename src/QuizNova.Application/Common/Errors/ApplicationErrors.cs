using QuizNova.Domain.Common.Results;

namespace QuizNova.Application.Common.Errors;

public static class ApplicationErrors
{
    public static readonly Error MissingRefreshToken = Error.Validation(
        code: "Auth.RefreshToken.Missing",
        description: "Refresh token is missing from the request.");

    public static readonly Error InvalidRefreshToken = Error.NotFound(
        code: "Auth.RefreshToken.Invalid",
        description: "Refresh token is invalid or does not exist.");

    public static readonly Error InvalidRoleForLogin = Error.Validation(
        code: "Auth.Role.Invalid",
        description: "User does not have the specified role.");

    public static readonly Error ExpiredOrRevokedRefreshToken = Error.Forbidden(
        code: "Auth.RefreshToken.ExpiredOrRevoked",
        description: "Refresh token has expired or has been revoked.");

    public static readonly Error ExpiredAccessTokenInvalid = Error.Validation(
        code: "Auth.ExpiredAccessToken.Invalid",
        description: "Expired access token is not valid.");

    public static readonly Error UserIdClaimInvalid = Error.Validation(
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

    public static Error QuizCourseNotFound(Guid courseId) =>
        Error.NotFound(
            code: "Quiz.Course.NotFound",
            description: $"Course with ID '{courseId}' was not found.");

    public static Error QuizCourseCompleted(Guid courseId) =>
        Error.Validation(
            code: "Quiz.Course.Completed",
            description: $"Cannot perform operations on a quiz for completed course with ID '{courseId}'.");

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
            code: "Enrollment.Enrollment.NotFound",
            description: $"Student with ID '{studentId}' is not enrolled in course '{courseId}'.");

    public static Error QuizAttemptAlreadyExists(Guid studentId, Guid quizId) =>
        Error.Conflict(
            code: "QuizAttempt.AlreadyExists",
            description: $"Student '{studentId}' already has an active attempt for quiz '{quizId}'.");

    public static Error QuizAttemptAlreadyCompleted(Guid studentId, Guid quizId) =>
        Error.Conflict(
            code: "QuizAttempt.AlreadyCompleted",
            description: $"Student '{studentId}' has already completed quiz '{quizId}'.");

    public static Error QuizCorrectChoiceNotFound(Guid questionId, Guid correctChoiceId) =>
        Error.Validation(
            code: "Quiz.Question.CorrectChoice.NotFound",
            description:
            $"Correct choice with ID '{correctChoiceId}' was not found for question with ID '{questionId}'.");

    public static Error QuizChoiceIdsMustBeUnique(Guid questionId) =>
        Error.Validation(
            code: "Quiz.Question.ChoiceIds.NotUnique",
            description: $"Choice IDs must be unique for question with ID '{questionId}'.");

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

    public static Error QuizAttemptNotFound(Guid attemptId) =>
        Error.NotFound(
            code: "QuizAttempt.NotFound",
            description: $"Quiz attempt with ID '{attemptId}' was not found.");

    public static Error AnswerNotFound(Guid answerId) =>
        Error.NotFound(
            code: "Answer.NotFound",
            description: $"Answer with ID '{answerId}' was not found.");

    public static Error CourseNotFound(Guid courseId) =>
        Error.NotFound(
            code: "Course.NotFound",
            description: $"Course with ID '{courseId}' was not found.");

    public static Error EnrollmentNotFound(Guid enrollmentId) =>
        Error.NotFound(
            code: "Enrollment.NotFound",
            description: $"Enrollment with ID '{enrollmentId}' was not found.");

    public static Error CourseChatRoomNotFound(Guid roomId) =>
        Error.NotFound(
            code: "CourseChatRoom.NotFound",
            description: $"Course chat room with ID '{roomId}' was not found.");

    public static Error MessageNotFound(Guid messageId) =>
        Error.NotFound(
            code: "Message.NotFound",
            description: $"Message with ID '{messageId}' was not found.");
}
