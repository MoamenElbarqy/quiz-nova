using MediatR;

using Microsoft.Extensions.Logging;

using QuizNova.Application.Common.Interfaces;
using QuizNova.Domain.Entities.Enrollments.Events;
using QuizNova.Domain.Entities.Users.Student;

namespace QuizNova.Application.Features.CourseChats.EventHandlers;

public sealed class RemoveStudentFromCourseChatRoomOnStudentDisenrolledHandler(
    IMongoDbContext mongoContext,
    ILogger<RemoveStudentFromCourseChatRoomOnStudentDisenrolledHandler> logger)
    : INotificationHandler<StudentDisenrolledEvent>
{
    public async Task Handle(StudentDisenrolledEvent notification, CancellationToken ct)
    {
        logger.LogInformation("Removing student {StudentId} from chatroom of course {CourseId}", notification.StudentId, notification.CourseId);

        var studentExists = await mongoContext.Users
            .Find(u => u.Id == notification.StudentId && u is Student)
            .AnyAsync(ct);

        if (!studentExists)
        {
            logger.LogWarning("Could not remove student from chatroom: student {StudentId} not found.", notification.StudentId);
            return;
        }

        var chatRoom = await mongoContext.CourseChatRooms
            .Find(r => r.CourseId == notification.CourseId)
            .FirstOrDefaultAsync(ct);

        if (chatRoom is null)
        {
            logger.LogWarning("Could not remove student from chatroom: chatroom for course {CourseId} not found.", notification.CourseId);
            return;
        }

        chatRoom.RemoveStudent(notification.StudentId);

        await mongoContext.CourseChatRooms.ReplaceOneAsync(r => r.Id == chatRoom.Id, chatRoom, cancellationToken: ct);

        logger.LogInformation("Successfully removed student {StudentId} from chatroom {RoomId}", notification.StudentId, chatRoom.Id);
    }
}
