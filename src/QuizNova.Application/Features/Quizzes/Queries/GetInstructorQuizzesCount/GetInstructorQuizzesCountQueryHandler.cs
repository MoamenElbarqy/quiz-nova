using MediatR;

using Microsoft.EntityFrameworkCore;

using QuizNova.Application.Common.Errors;
using QuizNova.Application.Common.Interfaces;
using QuizNova.Application.Features.Quizzes.DTOs;
using QuizNova.Domain.Common.Results;

namespace QuizNova.Application.Features.Quizzes.Queries.GetInstructorQuizzesCount;

public sealed class GetInstructorQuizzesCountQueryHandler(IAppDbContext dbContext)
    : IRequestHandler<GetInstructorQuizzesCountQuery, Result<QuizzesCountDto>>
{
    public async Task<Result<QuizzesCountDto>> Handle(GetInstructorQuizzesCountQuery request, CancellationToken ct)
    {
        var instructorExists = await dbContext.Instructors.AnyAsync(instructor => instructor.Id == request.InstructorId, ct);
        if (!instructorExists)
        {
            return ApplicationErrors.InstructorNotFound(request.InstructorId);
        }

        var quizzesCount = await dbContext.Quizzes.CountAsync(quiz => quiz.InstructorId == request.InstructorId, ct);

        return new QuizzesCountDto(quizzesCount);
    }
}

