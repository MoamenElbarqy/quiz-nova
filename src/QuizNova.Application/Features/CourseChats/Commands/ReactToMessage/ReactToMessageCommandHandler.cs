using MediatR;

using Microsoft.Extensions.Logging;

using QuizNova.Application.Common.Errors;
using QuizNova.Application.Common.Interfaces;
using QuizNova.Application.Features.CourseChats.DTOs;
using QuizNova.Application.Features.CourseChats.Mappers;
using QuizNova.Domain.Common.Results;
using QuizNova.Domain.Entities.CourseChats;

namespace QuizNova.Application.Features.CourseChats.Commands.ReactToMessage;

public sealed class ReactToMessageCommandHandler(
    IMongoDbContext mongoContext,
    IUser currentUser,
    ILogger<ReactToMessageCommandHandler> logger)
    : IRequestHandler<ReactToMessageCommand, Result<ReactDto>>
{
    public async Task<Result<ReactDto>> Handle(ReactToMessageCommand request, CancellationToken ct)
    {
        var userIdString = currentUser.Id;
        if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId))
        {
            return CourseChatErrors.CannotReact;
        }

        var room = await mongoContext.CourseChatRooms
            .Find(r => r.Id == request.RoomId)
            .FirstOrDefaultAsync(ct);

        if (room == null)
        {
            return ApplicationErrors.CourseChatRoomNotFound(request.RoomId);
        }

        if (!room.CanReact(userId))
        {
            return CourseChatErrors.CannotReact;
        }

        var message = room.Messages.FirstOrDefault(m => m.Id == request.MessageId);
        if (message == null)
        {
            return ApplicationErrors.MessageNotFound(request.MessageId);
        }

        var reactionResult = Reaction.Create(request.MessageId, userId, request.Emoji);
        if (reactionResult.IsError)
        {
            return reactionResult.Errors;
        }

        var reaction = reactionResult.Value;
        var addResult = message.AddReaction(reaction);
        if (addResult.IsError)
        {
            return addResult.Errors;
        }

        await mongoContext.CourseChatRooms.ReplaceOneAsync(r => r.Id == room.Id, room, cancellationToken: ct);

        logger.LogInformation("User {UserId} reacted to message {MessageId}", userId, request.MessageId);

        return reaction.ToDto();
    }
}
