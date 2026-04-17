using MediatR;

using QuizNova.Application.Features.Courses.DTOs;
using QuizNova.Domain.Common.Results;

namespace QuizNova.Application.Features.Courses.Queries.GetStudentCoursesCount;

public sealed record GetStudentCoursesCountQuery(Guid StudentId)
    : IRequest<Result<CoursesCountDto>>;

