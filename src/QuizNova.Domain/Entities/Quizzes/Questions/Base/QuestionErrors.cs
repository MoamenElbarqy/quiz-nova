using QuizNova.Domain.Common.Results;

namespace QuizNova.Domain.Entities.Quizzes.Questions.Base;

public static class QuestionErrors
{
    public static readonly Error QuizIdRequired =
        Error.Validation("Question_QuizId_Required", "Quiz ID is required.");

    public static readonly Error QuestionTextRequired =
        Error.Validation("Question_Text_Required", "Question text is required.");

    public static readonly Error DisplayOrderInvalid =
        Error.Validation("Question_DisplayOrder_Invalid", "Display order cannot be negative.");

    public static readonly Error MarksInvalid =
        Error.Validation("Question_Marks_Invalid", "Marks must be greater than zero.");
}
