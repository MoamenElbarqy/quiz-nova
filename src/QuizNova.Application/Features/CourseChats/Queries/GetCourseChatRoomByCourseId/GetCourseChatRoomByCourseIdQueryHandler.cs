using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using QuizNova.Application.Common.Errors;
using QuizNova.Application.Common.Interfaces;
using QuizNova.Application.Features.CourseChats.DTOs;
using QuizNova.Application.Features.CourseChats.Mappers;
using QuizNova.Application.Features.Students.Mappers;
using QuizNova.Application.Features.Users.Mappers;
using QuizNova.Domain.Common.Results;
using QuizNova.Domain.Entities.CourseChats;

namespace QuizNova.Application.Features.CourseChats.Queries.GetCourseChatRoomByCourseId;

public sealed class GetCourseChatRoomByCourseIdQueryHandler(
    IAppDbContext dbContext,
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

        var room = await dbContext.CourseChatRooms
            .AsNoTracking()
            .Include(r => r.Students)
            .ThenInclude(s => s.Enrollments)
            .Include(r => r.Messages)
            .ThenInclude(m => m.Reacts)
            .Include(r => r.Messages)
            .ThenInclude(m => m.Sender)
            .AsSplitQuery()
            .FirstOrDefaultAsync(r => r.CourseId == request.CourseId, ct);

        if (room == null)
        {
            logger.LogWarning("Course chatroom not found for course: {CourseId}", request.CourseId);
            return ApplicationErrors.CourseChatRoomNotFound(Guid.Empty);
        }

        if (!room.CanJoin(userId))
        {
            var isCourseInstructor = await dbContext.Courses
                .AnyAsync(c => c.Id == request.CourseId && c.InstructorId == userId, ct);

            if (isCourseInstructor)
            {
            }
            else
            {
                var isEnrolled = await dbContext.Enrollments
                    .AnyAsync(e => e.CourseId == request.CourseId && e.StudentId == userId, ct);

                if (!isEnrolled)
                {
                    logger.LogWarning("Access denied for user {UserId} in chatroom {RoomId}", userId, room.Id);
                    return CourseChatErrors.CannotJoin;
                }
            }
        }

        var students = room.Students.Select(s => s.ToStudentDto(s.Enrollments.Count())).ToList();

        var messages = room.Messages
            .OrderBy(m => m.CreatedAt)
            .Select(m => m.ToDto(
                m.Sender.ToDto(),
                m.Reacts.Select(r => r.ToDto()).ToList()))
            .ToList();

        var dto = new CourseChatRoomDto(
            room.Id,
            room.CourseId,
            room.InstructorId,
            students,
            messages);

        return dto;
    }
}
