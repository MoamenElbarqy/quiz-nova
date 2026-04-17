using MediatR;

using Microsoft.EntityFrameworkCore;

using QuizNova.Application.Common.Errors;
using QuizNova.Application.Common.Interfaces;
using QuizNova.Application.Features.Courses.DTOs;
using QuizNova.Domain.Common.Results;

namespace QuizNova.Application.Features.Courses.Queries.GetInstructorCoursesCount;

public sealed class GetInstructorCoursesCountQueryHandler(IAppDbContext dbContext)
    : IRequestHandler<GetInstructorCoursesCountQuery, Result<CoursesCountDto>>
{
    public async Task<Result<CoursesCountDto>> Handle(GetInstructorCoursesCountQuery request, CancellationToken ct)
    {
        var instructorExists = await dbContext.Instructors.AnyAsync(instructor => instructor.Id == request.InstructorId, ct);
        if (!instructorExists)
        {
            return ApplicationErrors.InstructorNotFound(request.InstructorId);
        }

        var courseCount = await dbContext.Courses.CountAsync(course => course.InstructorId == request.InstructorId, ct);

        return new CoursesCountDto(courseCount);
    }
}

