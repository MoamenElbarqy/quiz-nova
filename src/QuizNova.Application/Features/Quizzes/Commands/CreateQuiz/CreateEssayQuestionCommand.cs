namespace QuizNova.Application.Features.Quizzes.Commands.CreateQuiz;

public sealed record CreateEssayQuestionCommand(
    Guid Id,
    Guid QuizId,
    string QuestionText,
    int Marks)
    : CreateQuestionCommand(Id, QuizId, QuestionText, Marks);
