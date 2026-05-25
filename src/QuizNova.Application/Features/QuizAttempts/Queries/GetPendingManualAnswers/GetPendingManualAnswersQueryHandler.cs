using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using QuizNova.Application.Common.Errors;
using QuizNova.Application.Common.Interfaces;
using QuizNova.Application.Features.QuizAttempts.DTOs;
using QuizNova.Domain.Common.Results;
using QuizNova.Domain.Entities.QuizAttempts.Answers.ManuallyGradedAnswers;

namespace QuizNova.Application.Features.QuizAttempts.Queries.GetPendingManualAnswers;

public sealed class GetPendingManualAnswersQueryHandler(
    IAppDbContext dbContext,
    IUser user,
    ILogger<GetPendingManualAnswersQueryHandler> logger)
    : IRequestHandler<GetPendingManualAnswersQuery, Result<IReadOnlyList<PendingManualAnswersDto>>>
{
    public async Task<Result<IReadOnlyList<PendingManualAnswersDto>>> Handle(
        GetPendingManualAnswersQuery request,
        CancellationToken ct)
    {
        if (!Guid.TryParse(user.Id, out var instructorId))
        {
            return ApplicationErrors.UserIdClaimInvalid;
        }

        logger.LogInformation(
            "Fetching pending manually-graded answers for instructor {InstructorId}",
            instructorId);

        var result = await dbContext.QuizAttempts
            .Where(a =>
                a.Quiz!.InstructorId == instructorId &&
                a.StudentAnswers.OfType<ManuallyGradedAnswers>().Any(m => m.Score == null))
            .Select(a => new PendingManualAnswersDto(
                a.Id,
                a.StudentId,
                a.Student!.PersonalInformation.Name,
                a.Quiz!.Course!.Name,
                a.Quiz!.Title,
                a.SubmittedAt,
                a.StudentAnswers.OfType<ManuallyGradedAnswers>().Count(m => m.Score == null)))
            .AsNoTracking()
            .ToListAsync(ct);

        logger.LogInformation(
            "Found {Count} attempts with pending manual answers for instructor {InstructorId}",
            result.Count,
            instructorId);

        return result.AsReadOnly();
    }
}
