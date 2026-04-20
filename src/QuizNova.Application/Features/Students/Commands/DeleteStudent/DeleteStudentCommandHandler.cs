using MediatR;

using Microsoft.EntityFrameworkCore;

using QuizNova.Application.Common.Errors;
using QuizNova.Application.Common.Interfaces;
using QuizNova.Domain.Common.Results;

namespace QuizNova.Application.Features.Students.Commands.DeleteStudent;

public sealed class DeleteStudentCommandHandler(IAppDbContext dbContext)
    : IRequestHandler<DeleteStudentCommand, Result<Deleted>>
{
    public async Task<Result<Deleted>> Handle(DeleteStudentCommand request, CancellationToken ct)
    {
        var student = await dbContext.Students
            .FirstOrDefaultAsync(entity => entity.Id == request.Id, ct);

        if (student is null)
        {
            return ApplicationErrors.StudentNotFound(request.Id);
        }

        dbContext.Students.Remove(student);
        await dbContext.SaveChangesAsync(ct);

        return Result.Deleted;
    }
}
