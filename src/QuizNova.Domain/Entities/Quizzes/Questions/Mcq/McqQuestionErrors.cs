using QuizNova.Domain.Common.Results;

namespace QuizNova.Domain.Entities.Quizzes.Questions.Mcq;

public static class McqQuestionErrors
{
    public static readonly Error NumberOfChoicesInvalid =
        Error.Validation("McqQuestion_NumberOfChoices_Invalid", "Number of choices must be at least 2.");

    public static readonly Error CorrectChoiceIdRequired =
        Error.Validation("McqQuestion_CorrectChoiceId_Required", "Correct choice ID is required.");
}
