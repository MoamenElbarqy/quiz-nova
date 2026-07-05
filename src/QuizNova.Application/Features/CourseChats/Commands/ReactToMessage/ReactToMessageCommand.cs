using MediatR;

using QuizNova.Application.Features.CourseChats.DTOs;
using QuizNova.Domain.Common.Results;

namespace QuizNova.Application.Features.CourseChats.Commands.ReactToMessage;

public sealed record ReactToMessageCommand(
    Guid RoomId,
    Guid MessageId,
    string Emoji) : IRequest<Result<ReactDto>>;
