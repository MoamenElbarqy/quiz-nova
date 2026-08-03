using MediatR;

using Microsoft.Extensions.Logging;

using QuizNova.Application.Common.Errors;
using QuizNova.Application.Common.Interfaces;
using QuizNova.Application.Features.CourseChats.DTOs;
using QuizNova.Application.Features.CourseChats.Mappers;
using QuizNova.Application.Features.Users.Mappers;
using QuizNova.Domain.Common.Results;
using QuizNova.Domain.Entities.CourseChats;

namespace QuizNova.Application.Features.CourseChats.Commands.SendMessage;

public sealed class SendMessageCommandHandler(
    IMongoDbContext mongoContext,
    IUser currentUser,
    ILogger<SendMessageCommandHandler> logger)
    : IRequestHandler<SendMessageCommand, Result<MessageDto>>
{
    public async Task<Result<MessageDto>> Handle(SendMessageCommand request, CancellationToken ct)
    {
        var userIdString = currentUser.Id;
        if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId))
        {
            return CourseChatErrors.CannotSend;
        }

        var room = await mongoContext.CourseChatRooms
            .Find(r => r.Id == request.RoomId)
            .FirstOrDefaultAsync(ct);

        if (room == null)
        {
            return ApplicationErrors.CourseChatRoomNotFound(request.RoomId);
        }

        if (!room.CanSend(userId))
        {
            return CourseChatErrors.CannotSend;
        }

        var sendResult = room.SendMessage(userId, request.ReplyOnId, request.Content);
        if (sendResult.IsError)
        {
            return sendResult.Errors;
        }

        var message = sendResult.Value;
        await mongoContext.CourseChatRooms.ReplaceOneAsync(r => r.Id == room.Id, room, cancellationToken: ct);

        var senderUser = await mongoContext.Users
            .Find(u => u.Id == userId)
            .FirstOrDefaultAsync(ct);

        var senderDto = senderUser!.ToDto();

        logger.LogInformation("Message sent by user {UserId} in room {RoomId}", userId, request.RoomId);

        return message.ToDto(senderDto, []);
    }
}
