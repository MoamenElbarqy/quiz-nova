using System.Text.Json.Serialization;

using MediatR;

using QuizNova.Application.Features.Quizzes.Commands.CreateQuiz;
using QuizNova.Domain.Common.Results;

namespace QuizNova.Application.Features.Quizzes.Commands.UpdateQuestion;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "type")]
[JsonDerivedType(typeof(UpdateMcqCommand), "mcq")]
[JsonDerivedType(typeof(UpdateTfCommand), "tf")]
[JsonDerivedType(typeof(UpdateEssayCommand), "essay")]
public abstract record UpdateQuestionCommand(
    Guid QuizId,
    Guid QuestionId,
    string QuestionText,
    int DisplayOrder,
    int Marks)
    : IRequest<Result<Updated>>;

public sealed record UpdateMcqCommand(
    Guid QuizId,
    Guid QuestionId,
    string QuestionText,
    int DisplayOrder,
    int Marks,
    Guid CorrectChoiceId,
    IReadOnlyCollection<CreateChoiceCommand> Choices)
    : UpdateQuestionCommand(QuizId, QuestionId, QuestionText, DisplayOrder, Marks);

public sealed record UpdateTfCommand(
    Guid QuizId,
    Guid QuestionId,
    string QuestionText,
    int DisplayOrder,
    int Marks,
    bool CorrectChoice)
    : UpdateQuestionCommand(QuizId, QuestionId, QuestionText, DisplayOrder, Marks);

public sealed record UpdateEssayCommand(
    Guid QuizId,
    Guid QuestionId,
    string QuestionText,
    int DisplayOrder,
    int Marks,
    string? AnswerReference)
    : UpdateQuestionCommand(QuizId, QuestionId, QuestionText, DisplayOrder, Marks);
