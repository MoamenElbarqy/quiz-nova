using MediatR;

using Microsoft.EntityFrameworkCore;

using QuizNova.Application.Common.Errors;
using QuizNova.Application.Common.Interfaces;
using QuizNova.Application.Features.Quizzes.DTOs;
using QuizNova.Domain.Common.Results;
using QuizNova.Domain.Entities.Quizzes.Questions.Mcq;

namespace QuizNova.Application.Features.Quizzes.Queries.GetQuizById;

public sealed class GetQuizByIdQueryHandler(IAppDbContext dbContext)
    : IRequestHandler<GetQuizByIdQuery, Result<QuizDetailsDto>>
{
    public async Task<Result<QuizDetailsDto>> Handle(GetQuizByIdQuery request, CancellationToken ct)
    {
        var quiz = await dbContext.Quizzes
            .AsNoTracking()
            .Include(q => q.Questions)
            .Include(q => q.Questions.OfType<Mcq>())
            .ThenInclude(q => q.Choices)
            .FirstOrDefaultAsync(q => q.Id == request.QuizId, ct);

        if (quiz is null)
        {
            return ApplicationErrors.QuizNotFound(request.QuizId);
        }

        return quiz.ToQuizDetailsDto();
    }
}
