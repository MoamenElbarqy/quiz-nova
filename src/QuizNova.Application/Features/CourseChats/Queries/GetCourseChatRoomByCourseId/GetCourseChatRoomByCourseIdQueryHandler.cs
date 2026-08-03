using MediatR;

using Microsoft.Extensions.Logging;

using QuizNova.Application.Common.Errors;
using QuizNova.Application.Common.Interfaces;
using QuizNova.Application.Features.CourseChats.DTOs;
using QuizNova.Application.Features.CourseChats.Mappers;
using QuizNova.Application.Features.Students.DTOs;
using QuizNova.Application.Features.Students.Mappers;
using QuizNova.Application.Features.Users.Mappers;
using QuizNova.Domain.Common.Results;
using QuizNova.Domain.Entities.CourseChats;
using QuizNova.Domain.Entities.Users.Student;

namespace QuizNova.Application.Features.CourseChats.Queries.GetCourseChatRoomByCourseId;

public sealed class GetCourseChatRoomByCourseIdQueryHandler(
    IMongoDbContext mongoContext,
    IUser currentUser,
    ILogger<GetCourseChatRoomByCourseIdQueryHandler> logger)
    : IRequestHandler<GetCourseChatRoomByCourseIdQuery, Result<CourseChatRoomDto>>
{
    public async Task<Result<CourseChatRoomDto>> Handle(GetCourseChatRoomByCourseIdQuery request, CancellationToken ct)
    {
        logger.LogInformation("Retrieving chatroom for course ID: {CourseId}", request.CourseId);

        var userIdString = currentUser.Id;
        if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId))
        {
            return CourseChatErrors.CannotJoin;
        }

        var room = await mongoContext.CourseChatRooms
            .Find(r => r.CourseId == request.CourseId)
            .FirstOrDefaultAsync(ct);

        if (room == null)
        {
            logger.LogWarning("Course chatroom not found for course: {CourseId}", request.CourseId);
            return ApplicationErrors.CourseChatRoomNotFound(Guid.Empty);
        }

        if (!room.CanJoin(userId))
        {
            logger.LogWarning("Access denied for user {UserId} in chatroom {RoomId}", userId, room.Id);
            return CourseChatErrors.CannotJoin;
        }

        var studentIds = room.StudentIds.ToList();

        var studentDtos = new List<StudentDto>();
        if (studentIds.Count != 0)
        {
            var students = await mongoContext.Users
                .Find(u => studentIds.Contains(u.Id) && u is Student)
                .ToListAsync(ct);

            studentDtos =
            [
                .. students
                    .Cast<Student>()
                    .Select(s => s.ToStudentDto(0))
            ];
        }

        var messages = room.Messages
            .OrderBy(m => m.CreatedAt)
            .Select(m => m.ToDto(
                m.Sender.ToDto(),
                [.. m.Reacts.Select(r => r.ToDto())]))
            .ToList();

        var dto = new CourseChatRoomDto(
            room.Id,
            room.CourseId,
            room.InstructorId,
            studentDtos,
            messages);

        return dto;
    }
}
