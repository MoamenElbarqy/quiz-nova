namespace QuizNova.Application.Features.Quizzes.Commands.CreateQuiz;

public sealed record CreateTfCommand(
    Guid Id,
    Guid QuizId,
    string QuestionText,
    int Marks,
    bool CorrectChoice)
    : CreateQuestionCommand(Id, QuizId, QuestionText, Marks);
