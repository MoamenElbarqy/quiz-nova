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
}
