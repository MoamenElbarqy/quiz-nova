using MediatR;

using QuizNova.Application.Features.Quizzes.DTOs;
using QuizNova.Domain.Common.Results;

namespace QuizNova.Application.Features.Quizzes.Commands.CreateQuiz;

public sealed record CreateQuizCommand(
    Guid Id,
    string Title,
    Guid CourseId,
    Guid InstructorId,
    DateTimeOffset StartsAtUtc,
    DateTimeOffset EndsAtUtc,
    IReadOnlyCollection<CreateQuestionCommand> Questions)
    : IRequest<Result<QuizDto>>;
