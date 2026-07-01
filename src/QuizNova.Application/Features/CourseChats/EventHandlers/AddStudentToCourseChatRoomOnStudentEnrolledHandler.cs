using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using QuizNova.Application.Common.Interfaces;
using QuizNova.Domain.Entities.Courses.Events;

namespace QuizNova.Application.Features.CourseChats.EventHandlers;

public sealed class AddStudentToCourseChatRoomOnStudentEnrolledHandler(
    IAppDbContext dbContext,
    ILogger<AddStudentToCourseChatRoomOnStudentEnrolledHandler> logger)
    : INotificationHandler<StudentEnrolledEvent>
{
    public async Task Handle(StudentEnrolledEvent notification, CancellationToken ct)
    {
        logger.LogInformation("Adding student {StudentId} to chatroom of course {CourseId}", notification.StudentId, notification.CourseId);

        var student = await dbContext.Students
            .FirstOrDefaultAsync(u => u.Id == notification.StudentId, ct);

        if (student is null)
        {
            logger.LogWarning("Could not add student to chatroom: student {StudentId} not found.", notification.StudentId);
            return;
        }

        var chatRoom = await dbContext.CourseChatRooms
            .Include(r => r.Students)
            .FirstOrDefaultAsync(r => r.CourseId == notification.CourseId, ct);

        if (chatRoom is null)
        {
            logger.LogWarning("Could not add student to chatroom: chatroom for course {CourseId} not found.", notification.CourseId);
            return;
        }

        chatRoom.AddStudent(student);
        await dbContext.SaveChangesAsync(ct);

        logger.LogInformation("Successfully added student {StudentId} to chatroom {RoomId}", notification.StudentId, chatRoom.Id);
    }
}
