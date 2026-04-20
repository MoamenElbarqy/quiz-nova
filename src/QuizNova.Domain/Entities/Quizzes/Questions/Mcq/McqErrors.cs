using QuizNova.Domain.Common.Results;

namespace QuizNova.Domain.Entities.Quizzes.Questions.Mcq;

public static class McqErrors
{
    public static readonly Error NumberOfChoicesInvalid =
        Error.Validation("Mcq_NumberOfChoices_Invalid", "Number of choices must be at least 2.");

    public static readonly Error CorrectChoiceIdRequired =
        Error.Validation("Mcq_CorrectChoiceId_Required", "Correct choice ID is required.");
}
