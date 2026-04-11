namespace QuizNova.Application.Features.Quizzes.Commands.CreateQuiz;

public abstract record CreateQuestionCommand(
    Guid Id,
    Guid QuizId,
    string QuestionText,
    int Marks);
