namespace QuizNova.Application.Features.Quizzes.Commands.CreateQuiz;

public sealed record CreateChoiceCommand(
    Guid Id,
    Guid QuestionId,
    string Text,
    int DisplayOrder);
