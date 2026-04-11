using MediatR;

using QuizNova.Application.Features.Students.DTOs;
using QuizNova.Domain.Common.Results;

namespace QuizNova.Application.Features.Students.Queries.GetAllStudents;

public sealed record GetAllStudentsQuery()
    : IRequest<Result<List<StudentDto>>>;
