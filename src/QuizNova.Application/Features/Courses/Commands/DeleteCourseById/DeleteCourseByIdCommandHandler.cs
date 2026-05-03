using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using QuizNova.Application.Common.Errors;
using QuizNova.Application.Common.Interfaces;
using QuizNova.Domain.Common.Results;
using QuizNova.Domain.Entities.Courses.Events;

namespace QuizNova.Application.Features.Courses.Commands.DeleteCourseById;

public sealed class DeleteCourseByIdCommandHandler(
    IAppDbContext dbContext,
    ILogger<DeleteCourseByIdCommandHandler> logger)
    : IRequestHandler<DeleteCourseByIdCommand, Result<Deleted>>
{
    public async Task<Result<Deleted>> Handle(DeleteCourseByIdCommand request, CancellationToken ct)
    {
        logger.LogInformation("Deleting course with ID: {CourseId}", request.CourseId);

        var course = await dbContext.Courses
            .FirstOrDefaultAsync(c => c.Id == request.CourseId, ct);

        if (course is null)
        {
            logger.LogWarning("Course deletion failed: Course with ID {CourseId} not found", request.CourseId);
            return ApplicationErrors.CourseNotFound(request.CourseId);
        }

        dbContext.Courses.Remove(course);
        course.AddDomainEvent(new CourseDeletedEvent(course.Id));
        await dbContext.SaveChangesAsync(ct);

        logger.LogInformation("Successfully deleted course {CourseId}", request.CourseId);

        return Result.Deleted;
    }
}
