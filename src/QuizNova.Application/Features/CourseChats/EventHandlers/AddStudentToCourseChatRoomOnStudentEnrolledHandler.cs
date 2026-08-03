using MediatR;

using Microsoft.Extensions.Logging;

using QuizNova.Application.Common.Interfaces;
using QuizNova.Domain.Entities.Courses.Events;
using QuizNova.Domain.Entities.Users.Student;

namespace QuizNova.Application.Features.CourseChats.EventHandlers;

public sealed class AddStudentToCourseChatRoomOnStudentEnrolledHandler(
    IMongoDbContext mongoContext,
    ILogger<AddStudentToCourseChatRoomOnStudentEnrolledHandler> logger)
    : INotificationHandler<StudentEnrolledEvent>
{
    public async Task Handle(StudentEnrolledEvent notification, CancellationToken ct)
    {
        logger.LogInformation("Adding student {StudentId} to chatroom of course {CourseId}", notification.StudentId, notification.CourseId);

        var studentExists = await mongoContext.Users
            .Find(u => u.Id == notification.StudentId && u is Student)
            .AnyAsync(ct);

        if (!studentExists)
        {
            logger.LogWarning("Could not add student to chatroom: student {StudentId} not found.", notification.StudentId);
            return;
        }

        var chatRoom = await mongoContext.CourseChatRooms
            .Find(r => r.CourseId == notification.CourseId)
            .FirstOrDefaultAsync(ct);

        if (chatRoom is null)
        {
            logger.LogWarning("Could not add student to chatroom: chatroom for course {CourseId} not found.", notification.CourseId);
            return;
        }

        chatRoom.AddStudent(notification.StudentId);

        await mongoContext.CourseChatRooms.ReplaceOneAsync(r => r.Id == chatRoom.Id, chatRoom, cancellationToken: ct);

        logger.LogInformation("Successfully added student {StudentId} to chatroom {RoomId}", notification.StudentId, chatRoom.Id);
    }
}
