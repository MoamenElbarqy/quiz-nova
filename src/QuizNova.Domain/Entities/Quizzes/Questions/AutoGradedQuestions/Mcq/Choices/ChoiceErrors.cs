using QuizNova.Domain.Common.Results;

namespace QuizNova.Domain.Entities.Quizzes.Questions.AutoGradedQuestions.Mcq.Choices;

public static class ChoiceErrors
{
    public static readonly Error QuestionIdRequired =
        Error.Validation("Choice_QuestionId_Required", "Question ID is required.");

    public static readonly Error TextRequired =
        Error.Validation("Choice_Text_Required", "Choice text is required.");

    public static readonly Error DisplayOrderInvalid =
        Error.Validation("Choice_DisplayOrder_Invalid", "Display order cannot be negative.");

    public static readonly Error ChoiceTooShort =
        Error.Validation("Choice_Text_TooShort", "Choice text must be at least 3 characters long.");

    public static readonly Error ChoiceTooLong =
        Error.Validation("Choice_Text_TooLong", "Choice text cannot exceed 100 characters.");
}
