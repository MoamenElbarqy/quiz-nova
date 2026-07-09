using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using QuizNova.Application.Common.Errors;
using QuizNova.Application.Common.Interfaces;
using QuizNova.Application.Features.CourseChats.DTOs;
using QuizNova.Application.Features.CourseChats.Mappers;
using QuizNova.Domain.Common.Results;
using QuizNova.Domain.Entities.CourseChats;

namespace QuizNova.Application.Features.CourseChats.Commands.ReactToMessage;

public sealed class ReactToMessageCommandHandler(
    IAppDbContext dbContext,
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

        var room = await dbContext.CourseChatRooms
            .Include(r => r.Students)
            .FirstOrDefaultAsync(r => r.Id == request.RoomId, ct);

        if (room == null)
        {
            return ApplicationErrors.CourseChatRoomNotFound(request.RoomId);
        }

        if (!room.CanReact(userId))
        {
            var isInstructor = await dbContext.Courses
                .AnyAsync(c => c.Id == room.CourseId && c.InstructorId == userId, ct);

            if (!isInstructor)
            {
                var isEnrolled = await dbContext.Enrollments
                    .AnyAsync(e => e.CourseId == room.CourseId && e.StudentId == userId, ct);

                if (!isEnrolled)
                {
                    return CourseChatErrors.CannotReact;
                }
            }
        }

        var message = await dbContext.CourseChatRoomMessages
            .Include(m => m.Reacts)
            .FirstOrDefaultAsync(m => m.Id == request.MessageId && m.RoomId == request.RoomId, ct);

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

        dbContext.Reactions.Add(reaction);

        await dbContext.SaveChangesAsync(ct);

        logger.LogInformation("User {UserId} reacted to message {MessageId}", userId, request.MessageId);

        return reaction.ToDto();
    }
}
