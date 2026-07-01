using MediatR;

using QuizNova.Application.Features.CourseChats.DTOs;
using QuizNova.Domain.Common.Results;

namespace QuizNova.Application.Features.CourseChats.Queries.GetCourseChatRoomByCourseId;

public sealed record GetCourseChatRoomByCourseIdQuery(Guid CourseId)
    : IRequest<Result<CourseChatRoomDto>>;
