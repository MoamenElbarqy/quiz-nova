using MediatR;
using QuizNova.Application.Features.Courses.DTOs;
using QuizNova.Domain.Common.Results;

namespace QuizNova.Application.Features.Courses.Queries.GetInstructorCourses;

public record GetInstructorCoursesQuery(Guid InstructorId) : IRequest<Result<List<CourseDto>>>;
