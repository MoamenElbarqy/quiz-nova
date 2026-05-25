namespace QuizNova.Application.Features.Quizzes.Commands.CreateQuiz;

public sealed record CreateEssayCommand(
    string QuestionText,
    int Marks,
    string? AnswerReference)
    : CreateQuestionCommand(QuestionText, Marks);
