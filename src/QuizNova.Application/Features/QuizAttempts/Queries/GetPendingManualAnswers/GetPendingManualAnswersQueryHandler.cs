using MediatR;

using Microsoft.Extensions.Logging;

using QuizNova.Application.Common.Errors;
using QuizNova.Application.Common.Interfaces;
using QuizNova.Application.Common.Models;
using QuizNova.Application.Features.QuizAttempts.DTOs;
using QuizNova.Domain.Common.Results;
using QuizNova.Domain.Entities.QuizAttempts;
using QuizNova.Domain.Entities.QuizAttempts.Answers.Base;
using QuizNova.Domain.Entities.QuizAttempts.Answers.ManuallyGradedAnswers;
using QuizNova.Domain.Entities.Users.Student;

namespace QuizNova.Application.Features.QuizAttempts.Queries.GetPendingManualAnswers;

public sealed class GetPendingManualAnswersQueryHandler(
    IMongoDbContext mongoContext,
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

        var quizzes = await mongoContext.Quizzes
            .Find(q => q.InstructorId == instructorId)
            .ToListAsync(ct);

        var quizMap = quizzes.ToDictionary(q => q.Id);
        var instructorQuizIds = quizMap.Keys.ToList();

        var filter = Builders<QuizAttempt>.Filter.In(a => a.QuizId, instructorQuizIds) &
                     Builders<QuizAttempt>.Filter.ElemMatch("student_answers",
                         Builders<QuestionAnswer>.Filter
                             .OfType(
                                 Builders<ManuallyGradedAnswers>.Filter.Eq(m => m.Score, null)));

        var totalCount = (int)await mongoContext.QuizAttempts.CountDocumentsAsync(filter, cancellationToken: ct);

        var attempts = await mongoContext.QuizAttempts
            .Find(filter)
            .SortByDescending(a => a.SubmittedAt)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Limit(request.PageSize)
            .ToListAsync(ct);

        var studentIds = attempts.Select(a => a.StudentId).Distinct().ToList();

        var studentNames = new Dictionary<Guid, string>();
        if (studentIds.Count != 0)
        {
            var students = await mongoContext.Users
                .Find(u => studentIds.Contains(u.Id) && u is Student)
                .ToListAsync(ct);
            studentNames = students
                .Cast<Student>()
                .ToDictionary(s => s.Id, s => s.PersonalInformation.Name);
        }

        var courseIds = quizzes.Select(q => q.CourseId).Distinct().ToList();
        var courses = await mongoContext.Courses
            .Find(c => courseIds.Contains(c.Id))
            .ToListAsync(ct);
        var courseDict = courses.ToDictionary(c => c.Id, c => c.Name);

        var items = attempts.Select(a =>
        {
            var quiz = quizMap.GetValueOrDefault(a.QuizId);
            return new PendingManualAnswersDto(
                a.Id,
                a.StudentId,
                studentNames.GetValueOrDefault(a.StudentId, string.Empty),
                quiz != null ? courseDict.GetValueOrDefault(quiz.CourseId, string.Empty) : string.Empty,
                quiz?.Title ?? string.Empty,
                a.SubmittedAt ?? default,
                a.StudentAnswers.OfType<ManuallyGradedAnswers>().Count(m => m.Score == null));
        }).ToList();

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
