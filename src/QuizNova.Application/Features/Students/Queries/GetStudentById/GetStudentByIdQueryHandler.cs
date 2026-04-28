using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using QuizNova.Application.Common.Errors;
using QuizNova.Application.Common.Interfaces;
using QuizNova.Application.Features.Students.DTOs;
using QuizNova.Domain.Common.Results;

namespace QuizNova.Application.Features.Students.Queries.GetStudentById;

public sealed class GetStudentByIdQueryHandler(
    IAppDbContext dbContext,
    ILogger<GetStudentByIdQueryHandler> logger)
    : IRequestHandler<GetStudentByIdQuery, Result<StudentDto>>
{
    public async Task<Result<StudentDto>> Handle(GetStudentByIdQuery request, CancellationToken ct)
    {
        logger.LogInformation("Retrieving student with ID: {StudentId}", request.Id);

        var student = await dbContext.Students
            .AsNoTracking()
            .Select(student => new StudentDto(
                student.Id,
                student.PersonalInformation.Name,
                student.PersonalInformation.Email,
                student.PersonalInformation.Password,
                student.PersonalInformation.PhoneNumber,
                dbContext.StudentCourses.Count(studentCourse => studentCourse.StudentId == student.Id)))
            .FirstOrDefaultAsync(s => s.StudentId == request.Id, ct);

        if (student is null)
        {
            logger.LogWarning("Retrieval failed: Student with ID {StudentId} not found", request.Id);
            return ApplicationErrors.StudentNotFound(request.Id);
        }

        logger.LogInformation("Successfully retrieved student {StudentId}", request.Id);

        return student;
    }
}
