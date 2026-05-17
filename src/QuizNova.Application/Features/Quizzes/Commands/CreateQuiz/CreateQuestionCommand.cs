namespace QuizNova.Application.Features.Quizzes.Commands.CreateQuiz;

public abstract record CreateQuestionCommand(
    string QuestionText,
    int Marks);
