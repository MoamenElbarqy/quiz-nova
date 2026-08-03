using MediatR;

using Microsoft.Extensions.Logging;

using QuizNova.Application.Common.Interfaces;
using QuizNova.Domain.Entities.Courses.Events;

namespace QuizNova.Application.Features.CourseChats.EventHandlers;

public sealed class UpdateCourseChatRoomInstructorOnCourseUpdatedHandler(
    IMongoDbContext mongoContext,
    ILogger<UpdateCourseChatRoomInstructorOnCourseUpdatedHandler> logger)
    : INotificationHandler<CourseUpdatedEvent>
{
    public async Task Handle(CourseUpdatedEvent notification, CancellationToken ct)
    {
        logger.LogInformation("Updating instructor for chatroom of course {CourseId}", notification.Id);

        var course = await mongoContext.Courses
            .Find(c => c.Id == notification.Id)
            .FirstOrDefaultAsync(ct);

        if (course is null)
        {
            logger.LogWarning("Could not update chatroom instructor: course {CourseId} not found.", notification.Id);
            return;
        }

        var chatRoom = await mongoContext.CourseChatRooms
            .Find(r => r.CourseId == notification.Id)
            .FirstOrDefaultAsync(ct);

        if (chatRoom is null)
        {
            logger.LogWarning("Could not update chatroom instructor: chatroom for course {CourseId} not found.", notification.Id);
            return;
        }

        chatRoom.UpdateInstructor(course.InstructorId);
        await mongoContext.CourseChatRooms.ReplaceOneAsync(r => r.Id == chatRoom.Id, chatRoom, cancellationToken: ct);

        logger.LogInformation("Successfully updated instructor for chatroom {RoomId} to {InstructorId}", chatRoom.Id, course.InstructorId);
    }
}
