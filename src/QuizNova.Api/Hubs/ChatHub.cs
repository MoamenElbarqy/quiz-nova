using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

using MongoDB.Driver;

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
public class ChatHub(IMongoDbContext mongoContext, IUser currentUser, IMediator mediator) : Hub
{
    public async Task<Result<Success>> JoinRoom(Guid roomId)
    {
        var ct = Context.ConnectionAborted;
        var userIdString = currentUser.Id;
        if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId))
        {
            return CourseChatErrors.CannotJoin;
        }

        var room = await mongoContext.CourseChatRooms
            .Find(r => r.Id == roomId)
            .FirstOrDefaultAsync(ct);

        if (room == null)
        {
            return ApplicationErrors.CourseChatRoomNotFound(roomId);
        }

        if (!room.CanJoin(userId))
        {
            var isCourseInstructor = (await mongoContext.Courses
                                         .CountDocumentsAsync(c => c.Id == room.CourseId && c.InstructorId == userId,
                                             cancellationToken: ct)) >
                                     0;

            if (!isCourseInstructor)
            {
                var isEnrolled = (await mongoContext.Enrollments
                    .CountDocumentsAsync(e => e.CourseId == room.CourseId && e.StudentId == userId,
                        cancellationToken: ct)) > 0;

                if (!isEnrolled)
                {
                    return CourseChatErrors.CannotJoin;
                }
            }
        }

        await Groups.AddToGroupAsync(Context.ConnectionId, roomId.ToString(), ct);

        return Result.Success;
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
