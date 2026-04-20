using MediatR;

using Microsoft.EntityFrameworkCore;

using QuizNova.Application.Common.Interfaces;
using QuizNova.Application.Features.Instructor.DTOs;
using QuizNova.Domain.Common.Results;

namespace QuizNova.Application.Features.Instructor.Queries.GetAllInstructors;

public sealed class GetAllInstructorsQueryHandler(IAppDbContext dbContext)
    : IRequestHandler<GetAllInstructorsQuery, Result<List<InstructorDto>>>
{
    public async Task<Result<List<InstructorDto>>> Handle(GetAllInstructorsQuery request, CancellationToken ct)
    {
        var instructors = await dbContext.Instructors
            .AsNoTracking()
            .Select(instructor => new
            {
                instructor.Id,
                instructor.PersonalInformation.Name,
                instructor.PersonalInformation.Email,
                instructor.PersonalInformation.Password,
                instructor.PersonalInformation.PhoneNumber,
                CourseCount = dbContext.Courses.Count(course => course.InstructorId == instructor.Id),
                QuizCount = dbContext.Quizzes.Count(quiz => quiz.InstructorId == instructor.Id),
            })
            .ToListAsync(ct);

        var response = instructors
            .Select(instructor => new InstructorDto(
                instructor.Id,
                instructor.Name,
                instructor.Email,
                instructor.Password,
                instructor.PhoneNumber,
                instructor.CourseCount,
                instructor.QuizCount))
            .ToList();

        return response;
    }
}
