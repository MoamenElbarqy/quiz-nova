using MediatR;

using QuizNova.Application.Features.Quizzes.DTOs;
using QuizNova.Domain.Common.Results;

namespace QuizNova.Application.Features.Quizzes.Queries.GetAllQuizzes;

public sealed record GetAllQuizzesQuery()
    : IRequest<Result<List<QuizDto>>>;
