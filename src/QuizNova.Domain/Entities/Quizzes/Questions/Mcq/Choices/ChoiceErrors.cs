using QuizNova.Domain.Common.Results;

namespace QuizNova.Domain.Entities.Quizzes.Questions.Mcq.Choices;

public static class ChoiceErrors
{
    public static readonly Error QuestionIdRequired =
        Error.Validation("Choice_QuestionId_Required", "Question ID is required.");

    public static readonly Error TextRequired =
        Error.Validation("Choice_Text_Required", "Choice text is required.");

    public static readonly Error DisplayOrderInvalid =
        Error.Validation("Choice_DisplayOrder_Invalid", "Display order cannot be negative.");
}
