namespace QuizNova.Application.Features.Quizzes.Commands.CreateQuiz;

public sealed record CreateMultipleChoiceQuestionCommand(
    Guid Id,
    Guid QuizId,
    string QuestionText,
    int Marks,
    int NumberOfChoices,
    Guid CorrectChoiceId,
    IReadOnlyCollection<CreateChoiceCommand> Choices)
    : CreateQuestionCommand(Id, QuizId, QuestionText, Marks);
