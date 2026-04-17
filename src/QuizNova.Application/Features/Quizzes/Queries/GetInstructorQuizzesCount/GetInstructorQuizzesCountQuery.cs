using MediatR;

using QuizNova.Application.Features.Quizzes.DTOs;
using QuizNova.Domain.Common.Results;

namespace QuizNova.Application.Features.Quizzes.Queries.GetInstructorQuizzesCount;

public sealed record GetInstructorQuizzesCountQuery(Guid InstructorId)
    : IRequest<Result<QuizzesCountDto>>;

