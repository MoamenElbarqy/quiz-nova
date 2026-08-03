using MediatR;

using Microsoft.Extensions.Logging;

using QuizNova.Application.Common.Errors;
using QuizNova.Application.Common.Interfaces;
using QuizNova.Application.Features.Instructors.DTOs;
using QuizNova.Application.Features.Instructors.Mappers;
using QuizNova.Domain.Common.Results;
using QuizNova.Domain.Entities.Users.Instructors;

namespace QuizNova.Application.Features.Instructors.Queries.GetInstructorById;

public sealed class GetInstructorByIdQueryHandler(
    IMongoDbContext mongoContext,
    ILogger<GetInstructorByIdQueryHandler> logger)
    : IRequestHandler<GetInstructorByIdQuery, Result<InstructorDto>>
{
    public async Task<Result<InstructorDto>> Handle(GetInstructorByIdQuery request, CancellationToken ct)
    {
        logger.LogInformation("Retrieving instructor with ID: {InstructorId}", request.Id);

        var instructor = await mongoContext.Users
            .Find(u => u.Id == request.Id && u is Instructor)
            .FirstOrDefaultAsync(ct) as Instructor;

        if (instructor is null)
        {
            logger.LogWarning("Retrieval failed: Instructor with ID {InstructorId} not found", request.Id);
            return ApplicationErrors.InstructorNotFound(request.Id);
        }

        var coursesCount = (int)await mongoContext.Courses.CountDocumentsAsync(c => c.InstructorId == request.Id, cancellationToken: ct);
        var quizzesCount = (int)await mongoContext.Quizzes.CountDocumentsAsync(q => q.InstructorId == request.Id, cancellationToken: ct);

        logger.LogInformation("Successfully retrieved instructor {InstructorId}", request.Id);

        return instructor.ToInstructorDto(coursesCount, quizzesCount);
    }
}
