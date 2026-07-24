using MediatR;

using QuizNova.Application.Features.Quizzes.DTOs;
using QuizNova.Domain.Common.Results;

namespace QuizNova.Application.Features.Quizzes.Commands.CreateQuiz;

public sealed record CreateQuizCommand(
    string Title,
    Guid CourseId,
    DateTimeOffset StartsAtUtc,
    DateTimeOffset EndsAtUtc,
    IReadOnlyCollection<CreateQuestionCommand> Questions)
    : IRequest<Result<QuizDto>>;
