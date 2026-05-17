namespace QuizNova.Application.Features.Quizzes.Commands.CreateQuiz;

public sealed record CreateTfCommand(
    string QuestionText,
    int Marks,
    bool CorrectChoice)
    : CreateQuestionCommand(QuestionText, Marks);
