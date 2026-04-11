using MediatR;

using Microsoft.EntityFrameworkCore;

using QuizNova.Application.Common.Errors;
using QuizNova.Application.Common.Interfaces;
using QuizNova.Application.Features.QuizAttempts.DTOs;
using QuizNova.Application.Features.QuizAttempts.Mappers;
using QuizNova.Domain.Common.Results;

namespace QuizNova.Application.Features.QuizAttempts.Queries.GetStudentQuizAttempts;

public sealed class GetStudentQuizAttemptsQueryHandler(IAppDbContext dbContext)
    : IRequestHandler<GetStudentQuizAttemptsQuery, Result<List<QuizAttemptDto>>>
{
    public async Task<Result<List<QuizAttemptDto>>> Handle(GetStudentQuizAttemptsQuery request, CancellationToken ct)
    {
        var studentExists = await dbContext.Students
            .AsNoTracking()
            .AnyAsync(student => student.Id == request.StudentId, ct);

        if (!studentExists)
        {
            return ApplicationErrors.QuizAttemptStudentNotFound(request.StudentId);
        }

        var attempts = await dbContext.QuizAttempts
            .AsNoTracking()
            .Where(quizAttempt => quizAttempt.StudentId == request.StudentId)
            .Include(quizAttempt => quizAttempt.Quiz)
            .ThenInclude(quiz => quiz!.Questions)
            .Include(quizAttempt => quizAttempt.StudentAnswers)
            .OrderByDescending(quizAttempt => quizAttempt.StartedAt)
            .ToListAsync(ct);

        var response = attempts
            .Select(attempt => attempt.ToQuizAttemptDto())
            .ToList();

        return response;
    }
}
