using QuizNova.Domain.Common.Results;

namespace QuizNova.Domain.Entities.Quizzes.Questions.AutoGradedQuestions.Mcq;

public static class McqErrors
{
    public static readonly Error NumberOfChoicesInvalid =
        Error.Validation("Mcq_NumberOfChoices_Invalid", "Number of choices must be at least 2.");

    public static readonly Error CorrectChoiceIdRequired =
        Error.Validation("Mcq_CorrectChoiceId_Required", "Correct choice ID is required.");

    public static readonly Error TitleTooShort =
        Error.Validation("Mcq_Title_TooShort", "MCQ question text must be at least 3 characters long.");

    public static readonly Error TitleTooLong =
        Error.Validation("Mcq_Title_TooLong", "MCQ question text cannot exceed 500 characters.");

    public static Error CorrectChoiceNotFound(Guid questionId, Guid correctChoiceId) =>
        Error.Validation(
            code: "Quiz.Question.CorrectChoice.NotFound",
            description: $"Correct choice with ID '{correctChoiceId}' was not found for question with ID '{questionId}'.");

    public static Error ChoiceIdsMustBeUnique(Guid questionId) =>
        Error.Validation(
            code: "Quiz.Question.ChoiceIds.NotUnique",
            description: $"Choice IDs must be unique for question with ID '{questionId}'.");
}
