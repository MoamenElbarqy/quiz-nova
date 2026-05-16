namespace QuizNova.Application.Features.Quizzes.Commands.CreateQuiz;

public sealed record CreateMcqCommand(
    Guid Id,
    Guid QuizId,
    string QuestionText,
    int Marks,
    Guid CorrectChoiceId,
    IReadOnlyCollection<CreateChoiceCommand> Choices)
    : CreateQuestionCommand(Id, QuizId, QuestionText, Marks);
