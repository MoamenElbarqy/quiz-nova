using MediatR;

using Microsoft.EntityFrameworkCore;

using QuizNova.Application.Common.Errors;
using QuizNova.Application.Common.Interfaces;
using QuizNova.Application.Features.QuizAttempts.DTOs;
using QuizNova.Domain.Common.Results;

namespace QuizNova.Application.Features.QuizAttempts.Queries.GetStudentQuizAttemptsCount;

public sealed class GetStudentQuizAttemptsCountQueryHandler(IAppDbContext dbContext)
    : IRequestHandler<GetStudentQuizAttemptsCountQuery, Result<QuizAttemptsCountDto>>
{
    public async Task<Result<QuizAttemptsCountDto>> Handle(GetStudentQuizAttemptsCountQuery request, CancellationToken ct)
    {
        var studentExists = await dbContext.Students
            .AsNoTracking()
            .AnyAsync(student => student.Id == request.StudentId, ct);
        if (!studentExists)
        {
            return ApplicationErrors.StudentNotFound(request.StudentId);
        }

        var quizAttemptCount = await dbContext.QuizAttempts
            .AsNoTracking()
            .CountAsync(quizAttempt => quizAttempt.StudentId == request.StudentId, ct);

        return new QuizAttemptsCountDto(quizAttemptCount);
    }
}

