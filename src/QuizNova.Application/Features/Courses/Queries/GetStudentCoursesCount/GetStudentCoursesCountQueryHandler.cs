using MediatR;

using Microsoft.EntityFrameworkCore;

using QuizNova.Application.Common.Errors;
using QuizNova.Application.Common.Interfaces;
using QuizNova.Application.Features.Courses.DTOs;
using QuizNova.Domain.Common.Results;

namespace QuizNova.Application.Features.Courses.Queries.GetStudentCoursesCount;

public sealed class GetStudentCoursesCountQueryHandler(IAppDbContext dbContext)
    : IRequestHandler<GetStudentCoursesCountQuery, Result<CoursesCountDto>>
{
    public async Task<Result<CoursesCountDto>> Handle(GetStudentCoursesCountQuery request, CancellationToken ct)
    {
        var studentExists =
            await dbContext.Students.AsNoTracking().AnyAsync(student => student.Id == request.StudentId, ct);
        if (!studentExists)
        {
            return ApplicationErrors.StudentNotFound(request.StudentId);
        }

        var courseCount = await dbContext.StudentCourses
            .CountAsync(studentCourse => studentCourse.StudentId == request.StudentId, ct);

        return new CoursesCountDto(courseCount);
    }
}
