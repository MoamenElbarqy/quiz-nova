using MediatR;

using QuizNova.Application.Features.Quizzes.DTOs;
using QuizNova.Domain.Common.Results;

namespace QuizNova.Application.Features.Quizzes.Queries.GetStudentQuizzes;

public sealed record GetStudentQuizzesQuery(Guid StudentId)
    : IRequest<Result<StudentQuizzesDto>>;
