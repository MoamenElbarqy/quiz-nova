using MediatR;

using QuizNova.Domain.Common.Results;

namespace QuizNova.Application.Features.CourseChats.Commands.RemoveReaction;

public sealed record RemoveReactionCommand(
    Guid RoomId,
    Guid MessageId,
    Guid ReactionId) : IRequest<Result<Success>>;
