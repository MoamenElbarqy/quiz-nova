using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using QuizNova.Application.Common.Interfaces;
using QuizNova.Domain.Entities.CourseChats;
using QuizNova.Domain.Entities.Courses.Events;

namespace QuizNova.Application.Features.CourseChats.EventHandlers;

public sealed class CreateCourseChatRoomOnCourseCreatedHandler(
    IAppDbContext dbContext,
    ILogger<CreateCourseChatRoomOnCourseCreatedHandler> logger)
    : INotificationHandler<CourseCreatedEvent>
{
    public async Task Handle(CourseCreatedEvent notification, CancellationToken ct)
    {
        logger.LogInformation("Creating course chatroom for course {CourseId}", notification.Id);

        var course = await dbContext.Courses
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == notification.Id, ct);

        if (course is null)
        {
            logger.LogWarning("Could not create chatroom: course {CourseId} not found.", notification.Id);
            return;
        }

        var exists = await dbContext.CourseChatRooms.AnyAsync(r => r.CourseId == notification.Id, ct);
        if (exists)
        {
            return;
        }

        var chatRoomResult = CourseChatRoom.Create(course.Id, course.InstructorId);
        if (chatRoomResult.IsError)
        {
            logger.LogError("Failed to create chatroom: {Error}", chatRoomResult.TopError.Description);
            return;
        }

        await dbContext.CourseChatRooms.AddAsync(chatRoomResult.Value, ct);
        await dbContext.SaveChangesAsync(ct);

        logger.LogInformation("Successfully created chatroom {RoomId} for course {CourseId}", chatRoomResult.Value.Id, notification.Id);
    }
}
