using MediatR;

using Microsoft.EntityFrameworkCore;

using QuizNova.Application.Common.Errors;
using QuizNova.Application.Common.Interfaces;
using QuizNova.Application.Features.Courses.DTOs;
using QuizNova.Application.Features.Courses.Mappers;
using QuizNova.Domain.Common.Results;

namespace QuizNova.Application.Features.Courses.Queries.GetInstructorCourses;

public class GetInstructorCoursesQueryHandler : IRequestHandler<GetInstructorCoursesQuery, Result<List<CourseDto>>>
{
    private readonly IAppDbContext _dbContext;

    public GetInstructorCoursesQueryHandler(IAppDbContext dbContext)
    {
        this._dbContext = dbContext;
    }

    public async Task<Result<List<CourseDto>>> Handle(GetInstructorCoursesQuery request, CancellationToken ct)
    {
        var instructorExists = await _dbContext.Instructors.AnyAsync(i => i.Id == request.InstructorId, ct);
        if (!instructorExists)
        {
            return ApplicationErrors.InstructorNotFound(request.InstructorId);
        }

        var instructorCourses = await _dbContext.Courses
            .Where(c => c.InstructorId == request.InstructorId)
            .AsNoTracking()
            .ToListAsync(ct);

        if (!instructorCourses.Any())
        {
            return ApplicationErrors.NoCoursesForInstructor(request.InstructorId);
        }

        var courseDtos = instructorCourses.Select(c => c.ToCourseDto()).ToList();

        return courseDtos;
    }
}
