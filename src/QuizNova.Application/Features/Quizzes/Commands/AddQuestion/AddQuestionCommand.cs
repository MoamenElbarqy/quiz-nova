using MediatR;

using QuizNova.Application.Features.Quizzes.Commands.CreateQuiz;
using QuizNova.Application.Features.Quizzes.DTOs;
using QuizNova.Domain.Common.Results;

namespace QuizNova.Application.Features.Quizzes.Commands.AddQuestion;

public sealed record AddQuestionCommand(
    Guid QuizId,
    CreateQuestionCommand Question)
    : IRequest<Result<QuestionDto>>;
