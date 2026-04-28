using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using QuizNova.Application.Common.Errors;
using QuizNova.Application.Common.Interfaces;
using QuizNova.Application.Features.Instructor.DTOs;
using QuizNova.Domain.Common.Results;

namespace QuizNova.Application.Features.Instructor.Queries.GetInstructorById;

public sealed class GetInstructorByIdQueryHandler(
    IAppDbContext dbContext,
    ILogger<GetInstructorByIdQueryHandler> logger)
    : IRequestHandler<GetInstructorByIdQuery, Result<InstructorDto>>
{
    public async Task<Result<InstructorDto>> Handle(GetInstructorByIdQuery request, CancellationToken ct)
    {
        logger.LogInformation("Retrieving instructor with ID: {InstructorId}", request.Id);

        var instructor = await dbContext.Instructors
            .AsNoTracking()
            .Select(instructor => new
            {
                instructor.Id,
                instructor.PersonalInformation.Name,
                instructor.PersonalInformation.Email,
                instructor.PersonalInformation.Password,
                instructor.PersonalInformation.PhoneNumber,
                CoursesCount = dbContext.Courses.Count(course => course.InstructorId == instructor.Id),
                QuizzesCount = dbContext.Quizzes.Count(quiz => quiz.InstructorId == instructor.Id),
            })
            .FirstOrDefaultAsync(i => i.Id == request.Id, ct);

        if (instructor is null)
        {
            logger.LogWarning("Retrieval failed: Instructor with ID {InstructorId} not found", request.Id);
            return ApplicationErrors.InstructorNotFound(request.Id);
        }

        logger.LogInformation("Successfully retrieved instructor {InstructorId}", request.Id);

        return new InstructorDto(
            instructor.Id,
            instructor.Name,
            instructor.Email,
            instructor.Password,
            instructor.PhoneNumber,
            instructor.CoursesCount,
            instructor.QuizzesCount);
    }
}
