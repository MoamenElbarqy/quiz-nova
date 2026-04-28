using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using QuizNova.Application.Common.Errors;
using QuizNova.Application.Common.Interfaces;
using QuizNova.Application.Features.Courses.DTOs;
using QuizNova.Domain.Common.Results;

namespace QuizNova.Application.Features.Courses.Queries.GetStudentCoursesCount;

public sealed class GetStudentCoursesCountQueryHandler(
    IAppDbContext dbContext,
    ILogger<GetStudentCoursesCountQueryHandler> logger)
    : IRequestHandler<GetStudentCoursesCountQuery, Result<CoursesCountDto>>
{
    public async Task<Result<CoursesCountDto>> Handle(GetStudentCoursesCountQuery request, CancellationToken ct)
    {
        logger.LogInformation("Retrieving enrolled courses count for student with ID: {StudentId}", request.StudentId);

        var studentExists =
            await dbContext.Students.AsNoTracking().AnyAsync(student => student.Id == request.StudentId, ct);

        if (!studentExists)
        {
            logger.LogWarning("Retrieval failed: Student with ID {StudentId} not found", request.StudentId);
            return ApplicationErrors.StudentNotFound(request.StudentId);
        }

        var courseCount = await dbContext.StudentCourses
            .CountAsync(studentCourse => studentCourse.StudentId == request.StudentId, ct);

        logger.LogInformation("Successfully retrieved enrolled courses count for student {StudentId}: {Count}", request.StudentId, courseCount);

        return new CoursesCountDto(courseCount);
    }
}
