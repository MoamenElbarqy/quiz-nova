using MediatR;

using Microsoft.Extensions.Logging;

using QuizNova.Application.Common.Interfaces;
using QuizNova.Domain.Entities.CourseChats;
using QuizNova.Domain.Entities.Courses.Events;

namespace QuizNova.Application.Features.CourseChats.EventHandlers;

public sealed class CreateCourseChatRoomOnCourseCreatedHandler(
    IMongoDbContext mongoContext,
    ILogger<CreateCourseChatRoomOnCourseCreatedHandler> logger)
    : INotificationHandler<CourseCreatedEvent>
{
    public async Task Handle(CourseCreatedEvent notification, CancellationToken ct)
    {
        logger.LogInformation("Creating course chatroom for course {CourseId}", notification.Id);

        var course = await mongoContext.Courses
            .Find(c => c.Id == notification.Id)
            .FirstOrDefaultAsync(ct);

        if (course is null)
        {
            logger.LogWarning("Could not create chatroom: course {CourseId} not found.", notification.Id);
            return;
        }

        var exists = (await mongoContext.CourseChatRooms
            .CountDocumentsAsync(r => r.CourseId == notification.Id, cancellationToken: ct)) > 0;

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

        await mongoContext.CourseChatRooms.InsertOneAsync(chatRoomResult.Value, cancellationToken: ct);

        logger.LogInformation("Successfully created chatroom {RoomId} for course {CourseId}", chatRoomResult.Value.Id, notification.Id);
    }
}
