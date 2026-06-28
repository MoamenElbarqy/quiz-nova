using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using QuizNova.Application.Common.Interfaces;
using QuizNova.Domain.Entities.Courses.Events;

namespace QuizNova.Application.Features.CourseChats.EventHandlers;

public sealed class UpdateCourseChatRoomInstructorOnCourseUpdatedHandler(
    IAppDbContext dbContext,
    ILogger<UpdateCourseChatRoomInstructorOnCourseUpdatedHandler> logger)
    : INotificationHandler<CourseUpdatedEvent>
{
    public async Task Handle(CourseUpdatedEvent notification, CancellationToken ct)
    {
        logger.LogInformation("Updating instructor for chatroom of course {CourseId}", notification.Id);

        var course = await dbContext.Courses
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == notification.Id, ct);

        if (course is null)
        {
            logger.LogWarning("Could not update chatroom instructor: course {CourseId} not found.", notification.Id);
            return;
        }

        var chatRoom = await dbContext.CourseChatRooms
            .FirstOrDefaultAsync(r => r.CourseId == notification.Id, ct);

        if (chatRoom is null)
        {
            logger.LogWarning("Could not update chatroom instructor: chatroom for course {CourseId} not found.", notification.Id);
            return;
        }

        chatRoom.UpdateInstructor(course.InstructorId);
        await dbContext.SaveChangesAsync(ct);

        logger.LogInformation("Successfully updated instructor for chatroom {RoomId} to {InstructorId}", chatRoom.Id, course.InstructorId);
    }
}
