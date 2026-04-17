using MediatR;

using QuizNova.Application.Features.Courses.DTOs;
using QuizNova.Domain.Common.Results;

namespace QuizNova.Application.Features.Courses.Queries.GetInstructorCoursesCount;

public sealed record GetInstructorCoursesCountQuery(Guid InstructorId)
    : IRequest<Result<CoursesCountDto>>;

