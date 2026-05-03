using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using QuizNova.Application.Common.Errors;
using QuizNova.Application.Common.Interfaces;
using QuizNova.Domain.Common.Results;
using QuizNova.Domain.Entities.Users.Student.Events;

namespace QuizNova.Application.Features.Students.Commands.DeleteStudent;

public sealed class DeleteStudentCommandHandler(
    IAppDbContext dbContext,
    ILogger<DeleteStudentCommandHandler> logger)
    : IRequestHandler<DeleteStudentCommand, Result<Deleted>>
{
    public async Task<Result<Deleted>> Handle(DeleteStudentCommand request, CancellationToken ct)
    {
        logger.LogInformation("Deleting student with ID: {StudentId}", request.Id);

        var student = await dbContext.Students
            .FirstOrDefaultAsync(entity => entity.Id == request.Id, ct);

        if (student is null)
        {
            logger.LogWarning("Student deletion failed: Student with ID {StudentId} not found", request.Id);
            return ApplicationErrors.StudentNotFound(request.Id);
        }

        student.AddDomainEvent(new StudentDeletedEvent(student.Id));

        dbContext.Students.Remove(student);
        await dbContext.SaveChangesAsync(ct);

        logger.LogInformation("Successfully deleted student {StudentId}", request.Id);

        return Result.Deleted;
    }
}
