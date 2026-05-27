using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using QuizNova.Application.Common.Errors;
using QuizNova.Application.Common.Interfaces;
using QuizNova.Application.Common.Models;
using QuizNova.Application.Features.QuizAttempts.DTOs;
using QuizNova.Domain.Common.Results;
using QuizNova.Domain.Entities.QuizAttempts.Answers.ManuallyGradedAnswers;

namespace QuizNova.Application.Features.QuizAttempts.Queries.GetPendingManualAnswers;

public sealed class GetPendingManualAnswersQueryHandler(
    IAppDbContext dbContext,
    IUser user,
    ILogger<GetPendingManualAnswersQueryHandler> logger)
    : IRequestHandler<GetPendingManualAnswersQuery, Result<PaginatedList<PendingManualAnswersDto>>>
{
    public async Task<Result<PaginatedList<PendingManualAnswersDto>>> Handle(
        GetPendingManualAnswersQuery request,
        CancellationToken ct)
    {
        if (!Guid.TryParse(user.Id, out var instructorId))
        {
            return ApplicationErrors.UserIdClaimInvalid;
        }

        logger.LogInformation(
            "Fetching pending manually-graded answers for instructor {InstructorId} with PageNumber: {PageNumber}, PageSize: {PageSize}",
            instructorId, request.PageNumber, request.PageSize);

        var query = dbContext.QuizAttempts
            .Where(a =>
                a.Quiz!.InstructorId == instructorId &&
                a.StudentAnswers.OfType<ManuallyGradedAnswers>().Any(m => m.Score == null));

        var totalCount = await query.CountAsync(ct);

        var items = await query
            .OrderByDescending(a => a.SubmittedAt)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
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
            "Found {Count} total attempts with pending manual answers for instructor {InstructorId}. Returning {PageCount} for page {PageNumber}",
            totalCount,
            instructorId,
            items.Count,
            request.PageNumber);

        var result = new PaginatedList<PendingManualAnswersDto>(
            items,
            totalCount,
            request.PageNumber,
            request.PageSize);

        return result;
    }
}
