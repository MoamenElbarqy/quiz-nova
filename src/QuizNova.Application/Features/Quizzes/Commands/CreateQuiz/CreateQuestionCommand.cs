using System.Text.Json.Serialization;

namespace QuizNova.Application.Features.Quizzes.Commands.CreateQuiz;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "type")]
[JsonDerivedType(typeof(CreateMcqCommand), "mcq")]
[JsonDerivedType(typeof(CreateTfCommand), "tf")]
[JsonDerivedType(typeof(CreateEssayCommand), "essay")]

public abstract record CreateQuestionCommand(
    string QuestionText,
    int Marks);

public sealed record CreateMcqCommand(
    string QuestionText,
    int Marks,
    Guid CorrectChoiceId,
    IReadOnlyCollection<CreateChoiceCommand> Choices)
    : CreateQuestionCommand(QuestionText, Marks);

public sealed record CreateTfCommand(
    string QuestionText,
    int Marks,
    bool CorrectChoice)
    : CreateQuestionCommand(QuestionText, Marks);

public sealed record CreateEssayCommand(
    string QuestionText,
    int Marks,
    string? AnswerReference)
    : CreateQuestionCommand(QuestionText, Marks);

public sealed record CreateChoiceCommand(
    Guid Id,
    string Text,
    int DisplayOrder);
