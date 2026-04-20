using MediatR;

using QuizNova.Application.Features.Students.DTOs;
using QuizNova.Domain.Common.Results;

namespace QuizNova.Application.Features.Students.Commands.UpdateStudent;

public sealed record UpdateStudentCommand(
    Guid Id,
    string Name,
    string Email,
    string Password,
    string PhoneNumber)
    : IRequest<Result<StudentDto>>;
