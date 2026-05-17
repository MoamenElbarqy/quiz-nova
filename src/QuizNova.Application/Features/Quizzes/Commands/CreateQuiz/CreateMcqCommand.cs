namespace QuizNova.Application.Features.Quizzes.Commands.CreateQuiz;

public sealed record CreateMcqCommand(
    string QuestionText,
    int Marks,
    Guid CorrectChoiceId,
    IReadOnlyCollection<CreateChoiceCommand> Choices)
    : CreateQuestionCommand(QuestionText, Marks);
