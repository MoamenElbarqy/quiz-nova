using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using QuizNova.Application.Common.Errors;
using QuizNova.Application.Common.Interfaces;
using QuizNova.Application.Features.Courses.DTOs;
using QuizNova.Domain.Common.Results;

namespace QuizNova.Application.Features.Courses.Queries.GetStudentCoursesById;

public sealed class GetStudentCoursesByIdQueryHandler(
    IAppDbContext dbContext,
    ILogger<GetStudentCoursesByIdQueryHandler> logger)
    : IRequestHandler<GetStudentCoursesByIdQuery, Result<List<StudentCourseDto>>>
{
    public async Task<Result<List<StudentCourseDto>>> Handle(GetStudentCoursesByIdQuery request, CancellationToken ct)
    {
        logger.LogInformation("Retrieving courses for student with ID: {StudentId}", request.StudentId);

        var studentExists = await dbContext.Students.AnyAsync(s => s.Id == request.StudentId, ct);
        if (!studentExists)
        {
            logger.LogWarning("Retrieval failed: Student with ID {StudentId} not found", request.StudentId);
            return ApplicationErrors.StudentNotFound(request.StudentId);
        }

        var studentCourses = await dbContext.StudentCourses
            .Where(sc => sc.StudentId == request.StudentId)
            .AsNoTracking()
            .Select(sc => new StudentCourseDto(
                sc.CourseId,
                sc.Course!.Name,
                sc.Course.Instructor!.PersonalInformation.Name,
                dbContext.Quizzes.Count(quiz => quiz.CourseId == sc.CourseId),
                sc.EnrolledOnUtc))
            .ToListAsync(ct);

        logger.LogInformation("Successfully retrieved {Count} courses for student {StudentId}", studentCourses.Count, request.StudentId);

        return studentCourses;
    }
}
