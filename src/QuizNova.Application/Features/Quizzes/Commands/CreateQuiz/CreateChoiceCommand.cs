namespace QuizNova.Application.Features.Quizzes.Commands.CreateQuiz;

public sealed record CreateChoiceCommand(
    Guid Id,
    string Text,
    int DisplayOrder);
