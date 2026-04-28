using MediatR;

using QuizNova.Application.Features.Students.DTOs;
using QuizNova.Domain.Common.Results;

namespace QuizNova.Application.Features.Students.Queries.GetStudentById;

public sealed record GetStudentByIdQuery(Guid Id)
    : IRequest<Result<StudentDto>>;
