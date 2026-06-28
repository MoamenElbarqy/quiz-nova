using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using QuizNova.Application.Common.Interfaces;
using QuizNova.Domain.Entities.Enrollments.Events;

namespace QuizNova.Application.Features.CourseChats.EventHandlers;

public sealed class RemoveStudentFromCourseChatRoomOnEnrollmentDeletedHandler(
    IAppDbContext dbContext,
    ILogger<RemoveStudentFromCourseChatRoomOnEnrollmentDeletedHandler> logger)
    : INotificationHandler<EnrollmentDeletedEvent>
{
    public async Task Handle(EnrollmentDeletedEvent notification, CancellationToken ct)
    {
        logger.LogInformation("Removing student {StudentId} from chatroom of course {CourseId}", notification.StudentId, notification.CourseId);

        var student = await dbContext.Users
            .FirstOrDefaultAsync(u => u.Id == notification.StudentId, ct);

        if (student is null)
        {
            logger.LogWarning("Could not remove student from chatroom: student {StudentId} not found.", notification.StudentId);
            return;
        }

        var chatRoom = await dbContext.CourseChatRooms
            .Include(r => r.Students)
            .FirstOrDefaultAsync(r => r.CourseId == notification.CourseId, ct);

        if (chatRoom is null)
        {
            logger.LogWarning("Could not remove student from chatroom: chatroom for course {CourseId} not found.", notification.CourseId);
            return;
        }

        chatRoom.RemoveStudent(student);
        await dbContext.SaveChangesAsync(ct);

        logger.LogInformation("Successfully removed student {StudentId} from chatroom {RoomId}", notification.StudentId, chatRoom.Id);
    }
}
