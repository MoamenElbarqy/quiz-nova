namespace QuizNova.Domain.Entities.Quizzes.Questions;

public abstract record CreateQuestionArgs(
    string QuestionText,
    int Marks);

public sealed record CreateTfArgs(
    string QuestionText,
    int Marks,
    bool CorrectChoice)
    : CreateQuestionArgs(QuestionText, Marks);

public sealed record CreateMcqArgs(
    string QuestionText,
    int Marks,
    Guid CorrectChoiceId,
    IReadOnlyCollection<CreateChoiceArgs> Choices)
    : CreateQuestionArgs(QuestionText, Marks);

public sealed record CreateEssayArgs(
    string QuestionText,
    int Marks,
    string? AnswerReference)
    : CreateQuestionArgs(QuestionText, Marks);

public sealed record CreateChoiceArgs(
    Guid Id,
    string Text,
    int DisplayOrder);
