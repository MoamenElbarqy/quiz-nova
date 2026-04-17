using MediatR;

using QuizNova.Application.Features.Quizzes.DTOs;
using QuizNova.Domain.Common.Results;

namespace QuizNova.Application.Features.Quizzes.Queries.GetQuizById;

public sealed record GetQuizByIdQuery(Guid QuizId)
    : IRequest<Result<QuizDetailsDto>>;
