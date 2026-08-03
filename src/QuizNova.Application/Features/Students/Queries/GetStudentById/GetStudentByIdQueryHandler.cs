using MediatR;

using Microsoft.Extensions.Logging;

using QuizNova.Application.Common.Errors;
using QuizNova.Application.Common.Interfaces;
using QuizNova.Application.Features.Students.DTOs;
using QuizNova.Application.Features.Students.Mappers;
using QuizNova.Domain.Common.Results;
using QuizNova.Domain.Entities.Users.Student;

namespace QuizNova.Application.Features.Students.Queries.GetStudentById;

public sealed class GetStudentByIdQueryHandler(
    IMongoDbContext mongoContext,
    ILogger<GetStudentByIdQueryHandler> logger)
    : IRequestHandler<GetStudentByIdQuery, Result<StudentDto>>
{
    public async Task<Result<StudentDto>> Handle(GetStudentByIdQuery request, CancellationToken ct)
    {
        logger.LogInformation("Retrieving student with ID: {StudentId}", request.Id);

        var student = await mongoContext.Users
            .Find(u => u.Id == request.Id && u is Student)
            .FirstOrDefaultAsync(ct) as Student;

        if (student is null)
        {
            logger.LogWarning("Retrieval failed: Student with ID {StudentId} not found", request.Id);
            return ApplicationErrors.StudentNotFound(request.Id);
        }

        var enrollmentCount = (int)await mongoContext.Enrollments
            .CountDocumentsAsync(e => e.StudentId == request.Id, cancellationToken: ct);

        logger.LogInformation("Successfully retrieved student {StudentId}", request.Id);

        return student.ToStudentDto(enrollmentCount);
    }
}
