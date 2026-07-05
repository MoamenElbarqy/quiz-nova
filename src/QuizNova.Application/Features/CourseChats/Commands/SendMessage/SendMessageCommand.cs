using System.Text.Json;

using MediatR;

using QuizNova.Application.Features.CourseChats.DTOs;
using QuizNova.Domain.Common.Results;

namespace QuizNova.Application.Features.CourseChats.Commands.SendMessage;

public sealed record SendMessageCommand(
    Guid RoomId,
    Guid? ReplyOnId,
    JsonDocument Content) : IRequest<Result<MessageDto>>;
