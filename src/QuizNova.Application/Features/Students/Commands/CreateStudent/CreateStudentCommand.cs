using MediatR;

using QuizNova.Application.Features.Students.DTOs;
using QuizNova.Domain.Common.Results;

namespace QuizNova.Application.Features.Students.Commands.CreateStudent;

public sealed record CreateStudentCommand(
    Guid Id,
    string Name,
    string Email,
    string Password,
    string PhoneNumber,
    string Role)
    : IRequest<Result<StudentDto>>;
