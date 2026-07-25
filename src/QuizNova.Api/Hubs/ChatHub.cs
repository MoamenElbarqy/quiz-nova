using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

using QuizNova.Api.DTOs.Requests;
using QuizNova.Application.Common.Errors;
using QuizNova.Application.Common.Interfaces;
using QuizNova.Application.Features.CourseChats.Commands.ReactToMessage;
using QuizNova.Application.Features.CourseChats.Commands.RemoveReaction;
using QuizNova.Application.Features.CourseChats.Commands.SendMessage;
using QuizNova.Application.Features.CourseChats.DTOs;
using QuizNova.Domain.Common.Results;
using QuizNova.Domain.Entities.CourseChats;

namespace QuizNova.Api.Hubs;

[Authorize]
public class ChatHub(IAppDbContext dbContext, IUser currentUser, IMediator mediator) : Hub
{
    public async Task<Result<Success>> JoinRoom(Guid roomId)
    {
        var ct = Context.ConnectionAborted;
        var userIdString = currentUser.Id;
        if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId))
        {
            return CourseChatErrors.CannotJoin;
        }

        var room = await dbContext.CourseChatRooms
            .Include(r => r.Students)
            .FirstOrDefaultAsync(r => r.Id == roomId, ct);

        if (room == null)
        {
            return ApplicationErrors.CourseChatRoomNotFound(roomId);
        }

        if (!room.CanJoin(userId))
        {
            var isCourseInstructor = await dbContext.Courses
                .AnyAsync(c => c.Id == room.CourseId && c.InstructorId == userId, ct);

            if (!isCourseInstructor)
            {
                var isEnrolled = await dbContext.Enrollments
                    .AnyAsync(e => e.CourseId == room.CourseId && e.StudentId == userId, ct);

                if (!isEnrolled)
                {
                    return CourseChatErrors.CannotJoin;
                }
            }
        }

        await Groups.AddToGroupAsync(Context.ConnectionId, roomId.ToString(), ct);

        return Result.Success;
    }

    public async Task LeaveRoom(Guid roomId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomId.ToString());
    }

    public async Task<Result<MessageDto>> SendMessage(Guid roomId, SendMessageRequest request)
    {
        var ct = Context.ConnectionAborted;
        var command = new SendMessageCommand(roomId, request.ReplyOnId, request.Content);
        var result = await mediator.Send(command, ct);

        if (result.IsSuccess)
        {
            await Clients.Group(roomId.ToString()).SendAsync("ReceiveMessage", result.Value, ct);
        }

        return result;
    }

    public async Task<Result<ReactDto>> ReactToMessage(Guid roomId, ReactOnAMessageRequest request)
    {
        var ct = Context.ConnectionAborted;
        var command = new ReactToMessageCommand(roomId, request.MessageId, request.Emoji);
        var result = await mediator.Send(command, ct);

        if (result.IsSuccess)
        {
            await Clients.Group(roomId.ToString()).SendAsync("ReceiveReaction", result.Value, ct);
        }

        return result;
    }

    public async Task<Result<Success>> RemoveReaction(Guid roomId, Guid messageId, Guid reactionId)
    {
        var ct = Context.ConnectionAborted;
        var command = new RemoveReactionCommand(roomId, messageId, reactionId);
        var result = await mediator.Send(command, ct);

        if (result.IsSuccess)
        {
            await Clients.Group(roomId.ToString()).SendAsync("ReceiveReactionRemoved", reactionId, ct);
        }

        return result;
    }
}
