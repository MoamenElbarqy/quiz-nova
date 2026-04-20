using QuizNova.Domain.Common.Results;

namespace QuizNova.Domain.Entities.QuizAttempts;

public static class QuizAttemptErrors
{
    public static readonly Error AttemptIdRequired =
        Error.Validation("QuizAttempt_Id_Required", "Quiz attempt ID is required.");

    public static readonly Error StudentIdRequired =
        Error.Validation("QuizAttempt_StudentId_Required", "Student ID is required.");

    public static readonly Error QuizIdRequired =
        Error.Validation("QuizAttempt_QuizId_Required", "Quiz ID is required.");

    public static readonly Error StartedAtRequired =
        Error.Validation("QuizAttempt_StartedAt_Required", "Start time is required.");

    public static readonly Error SubmittedAtRequired =
        Error.Validation("QuizAttempt_SubmittedAt_Required", "Submission time is required.");

    public static readonly Error SubmittedAtInvalid =
        Error.Validation("QuizAttempt_SubmittedAt_Invalid", "Submission time cannot be earlier than start time.");

    public static readonly Error QuestionAnswersRequired =
        Error.Validation("QuizAttempt_QuestionAnswers_Required", "At least one question answer is required.");

    public static readonly Error DuplicateQuestionAnswers =
        Error.Validation("QuizAttempt_QuestionAnswers_Duplicate", "Duplicate question answers are not allowed.");

    public static Error QuizIdMismatch(Guid expectedQuizId, Guid actualQuizId) =>
        Error.Validation(
            "QuizAttempt_QuizId_Mismatch",
            $"Quiz ID '{actualQuizId}' does not match quiz aggregate ID '{expectedQuizId}'.");

    public static Error SubmittedAtAfterQuizEnd(DateTimeOffset quizEndTimeUtc) =>
        Error.Validation(
            "QuizAttempt_SubmittedAt_AfterQuizEnd",
            $"Submission time must be before or equal to quiz end time '{quizEndTimeUtc:O}'.");

    public static Error StartedAtBeforeQuizStart(DateTimeOffset quizStartTimeUtc) =>
        Error.Validation(
            "QuizAttempt_StartedAt_BeforeQuizStart",
            $"Start time must be after or equal to quiz start time '{quizStartTimeUtc:O}'.");

    public static Error TooManyQuestionAnswers(int providedAnswers, int questionCount) =>
        Error.Validation(
            "QuizAttempt_QuestionAnswers_TooMany",
            $"Provided '{providedAnswers}' answers but quiz has only '{questionCount}' questions.");

    public static Error QuestionNotFoundInQuiz(Guid questionId, Guid quizId) =>
        Error.Validation(
            "QuizAttempt_Question_NotFoundInQuiz",
            $"Question '{questionId}' does not belong to quiz '{quizId}'.");

    public static Error QuestionTypeMismatch(Guid questionId, string expectedAnswerType) =>
        Error.Validation(
            "QuizAttempt_Question_TypeMismatch",
            $"Question '{questionId}' does not match expected answer type '{expectedAnswerType}'.");

    public static Error AnswerQuizAttemptMismatch(Guid questionId, Guid expectedAttemptId, Guid actualAttemptId) =>
        Error.Validation(
            "QuizAttempt_Answer_QuizAttempt_Mismatch",
            $"Answer for question '{questionId}' references attempt '{actualAttemptId}' but expected '{expectedAttemptId}'.");

    public static Error AnswerStudentMismatch(Guid questionId, Guid expectedStudentId, Guid actualStudentId) =>
        Error.Validation(
            "QuizAttempt_Answer_Student_Mismatch",
            $"Answer for question '{questionId}' references student '{actualStudentId}' but expected '{expectedStudentId}'.");
}
