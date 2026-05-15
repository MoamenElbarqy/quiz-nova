using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using QuizNova.Application.Common.Errors;
using QuizNova.Application.Common.Interfaces;
using QuizNova.Application.Features.Instructors.DTOs;
using QuizNova.Application.Features.Instructors.Mappers;
using QuizNova.Domain.Common.Results;

namespace QuizNova.Application.Features.Instructors.Queries.GetInstructorById;

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
            .Include(i => i.Courses)
            .Include(i => i.Quizzes)
            .FirstOrDefaultAsync(i => i.Id == request.Id, ct);

        if (instructor is null)
        {
            logger.LogWarning("Retrieval failed: Instructor with ID {InstructorId} not found", request.Id);
            return ApplicationErrors.InstructorNotFound(request.Id);
        }

        logger.LogInformation("Successfully retrieved instructor {InstructorId}", request.Id);

        return instructor.ToInstructorDto(instructor.Courses.Count(), instructor.Quizzes.Count());
    }
}
