using MediatR;

using Microsoft.EntityFrameworkCore;

using QuizNova.Application.Common.Errors;
using QuizNova.Application.Common.Interfaces;
using QuizNova.Domain.Common.Results;

namespace QuizNova.Application.Features.Instructor.Commands.DeleteInstructor;

public sealed class DeleteInstructorCommandHandler(IAppDbContext dbContext)
    : IRequestHandler<DeleteInstructorCommand, Result<Deleted>>
{
    public async Task<Result<Deleted>> Handle(DeleteInstructorCommand request, CancellationToken ct)
    {
        var instructor = await dbContext.Instructors
            .FirstOrDefaultAsync(entity => entity.Id == request.Id, ct);

        if (instructor is null)
        {
            return ApplicationErrors.InstructorNotFound(request.Id);
        }

        dbContext.Instructors.Remove(instructor);
        await dbContext.SaveChangesAsync(ct);

        return Result.Deleted;
    }
}
