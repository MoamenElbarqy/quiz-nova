using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using QuizNova.Application.Common.Errors;
using QuizNova.Application.Common.Interfaces;
using QuizNova.Domain.Common.Results;
using QuizNova.Domain.Entities.CourseChats;

namespace QuizNova.Application.Features.CourseChats.Commands.RemoveReaction;

public sealed class RemoveReactionCommandHandler(
    IAppDbContext dbContext,
    IUser currentUser,
    ILogger<RemoveReactionCommandHandler> logger)
    : IRequestHandler<RemoveReactionCommand, Result<Success>>
{
    public async Task<Result<Success>> Handle(RemoveReactionCommand request, CancellationToken ct)
    {
        var userIdString = currentUser.Id;
        if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId))
        {
            return CourseChatErrors.CannotReact;
        }

        var room = await dbContext.CourseChatRooms
            .Include(r => r.Students)
            .FirstOrDefaultAsync(r => r.Id == request.RoomId, ct);

        if (room == null)
        {
            return ApplicationErrors.CourseChatRoomNotFound(request.RoomId);
        }

        if (!room.CanReact(userId))
        {
            return CourseChatErrors.CannotReact;
        }

        var message = await dbContext.CourseChatRoomMessages
            .Include(m => m.Reacts)
            .FirstOrDefaultAsync(m => m.Id == request.MessageId && m.RoomId == request.RoomId, ct);

        if (message == null)
        {
            return ApplicationErrors.MessageNotFound(request.MessageId);
        }

        var reaction = message.Reacts.FirstOrDefault(r => r.Id == request.ReactionId);
        if (reaction == null)
        {
            return CourseChatErrors.ReactionNotFound;
        }

        if (reaction.ReactorId != userId)
        {
            return CourseChatErrors.CannotReact;
        }

        var removeResult = message.RemoveReaction(request.ReactionId);
        if (removeResult.IsError)
        {
            return removeResult.Errors;
        }

        await dbContext.SaveChangesAsync(ct);

        logger.LogInformation("User {UserId} removed reaction from message {MessageId}", userId, request.MessageId);

        return Result.Success;
    }
}
