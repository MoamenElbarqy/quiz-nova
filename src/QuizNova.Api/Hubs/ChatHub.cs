using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

using QuizNova.Api.DTOs.Requests;
using QuizNova.Application.Common.Errors;
using QuizNova.Application.Common.Interfaces;
using QuizNova.Application.Features.CourseChats.DTOs;
using QuizNova.Application.Features.CourseChats.Mappers;
using QuizNova.Domain.Common.Results;
using QuizNova.Domain.Entities.CourseChats;

namespace QuizNova.Api.Hubs;

[Authorize]
public class ChatHub(IAppDbContext dbContext, IUser currentUser) : Hub
{
    public async Task<Result<Success>> JoinRoom(Guid roomId)
    {
        var userIdString = currentUser.Id;
        if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId))
        {
            return CourseChatErrors.CannotJoin;
        }

        var room = await dbContext.CourseChatRooms
            .Include(r => r.Students)
            .FirstOrDefaultAsync(r => r.Id == roomId);

        if (room == null)
        {
            return ApplicationErrors.CourseChatRoomNotFound(roomId);
        }

        if (!room.CanJoin(userId))
        {
            return CourseChatErrors.CannotJoin;
        }

        await Groups.AddToGroupAsync(Context.ConnectionId, roomId.ToString());

        return Result.Success;
    }

    public async Task LeaveRoom(Guid roomId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomId.ToString());
    }

    public async Task<Result<MessageDto>> SendMessage(Guid roomId, SendMessageRequest request)
    {
        var userIdString = currentUser.Id;
        if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId))
        {
            return CourseChatErrors.CannotSend;
        }

        var room = await dbContext.CourseChatRooms
            .Include(r => r.Students)
            .FirstOrDefaultAsync(r => r.Id == roomId);

        if (room == null)
        {
            return ApplicationErrors.CourseChatRoomNotFound(roomId);
        }

        if (!room.CanSend(userId))
        {
            return CourseChatErrors.CannotSend;
        }

        var messageResult = Message.Create(roomId, userId, request.ReplyOnId, request.Content);
        if (messageResult.IsError)
        {
            return messageResult.Errors;
        }

        var message = messageResult.Value;
        await dbContext.CourseChatRoomMessages.AddAsync(message);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var messageDto = message.ToDto();
        await Clients.Group(roomId.ToString()).SendAsync("ReceiveMessage", messageDto);

        return messageDto;
    }

    public async Task<Result<ReactDto>> ReactToMessage(Guid roomId, ReactOnAMessageRequest request)
    {
        var userIdString = currentUser.Id;
        if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId))
        {
            return CourseChatErrors.CannotReact;
        }

        var room = await dbContext.CourseChatRooms
            .Include(r => r.Students)
            .FirstOrDefaultAsync(r => r.Id == roomId);

        if (room == null)
        {
            return ApplicationErrors.CourseChatRoomNotFound(roomId);
        }

        if (!room.CanReact(userId))
        {
            return CourseChatErrors.CannotReact;
        }

        var message = await dbContext.CourseChatRoomMessages
            .Include(m => m.Reacts)
            .FirstOrDefaultAsync(m => m.Id == request.MessageId && m.RoomId == roomId);

        if (message == null)
        {
            return ApplicationErrors.MessageNotFound(request.MessageId);
        }

        var reactResult = React.Create(request.MessageId, userId, request.Emoji);
        if (reactResult.IsError)
        {
            return reactResult.Errors;
        }

        var react = reactResult.Value;
        var addResult = message.AddReaction(react);
        if (addResult.IsError)
        {
            return addResult.Errors;
        }

        if (dbContext is DbContext efDbContext)
        {
            efDbContext.Add(react);
        }

        await dbContext.SaveChangesAsync(CancellationToken.None);

        var reactDto = react.ToDto();
        await Clients.Group(roomId.ToString()).SendAsync("ReceiveReaction", reactDto);

        return reactDto;
    }

    public async Task<Result<Success>> RemoveReaction(Guid roomId, Guid messageId, Guid reactionId)
    {
        var userIdString = currentUser.Id;
        if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId))
        {
            return CourseChatErrors.CannotReact;
        }

        var room = await dbContext.CourseChatRooms
            .Include(r => r.Students)
            .FirstOrDefaultAsync(r => r.Id == roomId);

        if (room == null)
        {
            return ApplicationErrors.CourseChatRoomNotFound(roomId);
        }

        if (!room.CanReact(userId))
        {
            return CourseChatErrors.CannotReact;
        }

        var message = await dbContext.CourseChatRoomMessages
            .Include(m => m.Reacts)
            .FirstOrDefaultAsync(m => m.Id == messageId && m.RoomId == roomId);

        if (message == null)
        {
            return ApplicationErrors.MessageNotFound(messageId);
        }

        var reaction = message.Reacts.FirstOrDefault(r => r.Id == reactionId);
        if (reaction == null)
        {
            return CourseChatErrors.ReactionNotFound;
        }

        if (reaction.ReactorId != userId)
        {
            return CourseChatErrors.CannotReact;
        }

        var removeResult = message.RemoveReaction(reactionId);
        if (removeResult.IsError)
        {
            return removeResult.Errors;
        }

        await dbContext.SaveChangesAsync(CancellationToken.None);

        var reactDto = reaction.ToDto();
        await Clients.Group(roomId.ToString()).SendAsync("ReceiveReactionRemoved", reactDto);

        return Result.Success;
    }
}
