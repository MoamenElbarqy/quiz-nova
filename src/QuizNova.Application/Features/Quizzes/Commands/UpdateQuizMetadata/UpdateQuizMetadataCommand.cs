using MediatR;

using QuizNova.Domain.Common.Results;

namespace QuizNova.Application.Features.Quizzes.Commands.UpdateQuizMetadata;

public sealed record UpdateQuizMetadataCommand(
    Guid QuizId,
    string Title,
    DateTimeOffset StartsAtUtc,
    DateTimeOffset EndsAtUtc)
    : IRequest<Result<Updated>>;
