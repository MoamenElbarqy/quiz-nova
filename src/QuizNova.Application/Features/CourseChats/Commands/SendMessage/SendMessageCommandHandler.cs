using MediatR;

using Microsoft.EntityFrameworkCore;
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
    IAppDbContext dbContext,
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

        var room = await dbContext.CourseChatRooms
            .Include(r => r.Students)
            .FirstOrDefaultAsync(r => r.Id == request.RoomId, ct);

        if (room == null)
        {
            return ApplicationErrors.CourseChatRoomNotFound(request.RoomId);
        }

        if (!room.CanSend(userId))
        {
            var isInstructor = await dbContext.Courses
                .AnyAsync(c => c.Id == room.CourseId && c.InstructorId == userId, ct);

            if (!isInstructor)
            {
                var isEnrolled = await dbContext.Enrollments
                    .AnyAsync(e => e.CourseId == room.CourseId && e.StudentId == userId, ct);

                if (!isEnrolled)
                {
                    return CourseChatErrors.CannotSend;
                }
            }
        }

        var messageResult = Message.Create(request.RoomId, userId, request.ReplyOnId, request.Content);
        if (messageResult.IsError)
        {
            return messageResult.Errors;
        }

        var message = messageResult.Value;
        await dbContext.CourseChatRoomMessages.AddAsync(message, ct);
        await dbContext.SaveChangesAsync(ct);

        var senderUser = await dbContext.Users
            .AsNoTracking()
            .FirstAsync(u => u.Id == userId, ct);

        var senderDto = senderUser.ToDto();

        logger.LogInformation("Message sent by user {UserId} in room {RoomId}", userId, request.RoomId);

        return message.ToDto(senderDto, []);
    }
}
