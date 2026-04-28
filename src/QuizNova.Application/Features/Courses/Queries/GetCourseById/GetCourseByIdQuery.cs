using MediatR;

using QuizNova.Application.Features.Courses.DTOs;
using QuizNova.Domain.Common.Results;

namespace QuizNova.Application.Features.Courses.Queries.GetCourseById;

public sealed record GetCourseByIdQuery(Guid CourseId) : IRequest<Result<CourseDto>>;
