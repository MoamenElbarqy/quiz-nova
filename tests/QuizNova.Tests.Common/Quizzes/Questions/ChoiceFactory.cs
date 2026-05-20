using QuizNova.Domain.Common.Results;
using QuizNova.Domain.Entities.Quizzes.Questions.AutoGradedQuestions.Mcq.Choices;

namespace QuizNova.Tests.Common.Quizzes.Questions;

public static class ChoiceFactory
{
    public static Result<Choice> CreateChoice(
        Guid? id = null,
        Guid? questionId = null,
        string text = "Test Choice Option",
        int displayOrder = 1)
    {
        return Choice.Create(
            id ?? Guid.NewGuid(),
            questionId ?? Guid.NewGuid(),
            text,
            displayOrder);
    }
}
