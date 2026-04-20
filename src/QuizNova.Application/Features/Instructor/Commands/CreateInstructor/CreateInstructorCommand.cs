using MediatR;

using QuizNova.Application.Features.Instructor.DTOs;
using QuizNova.Domain.Common.Results;

namespace QuizNova.Application.Features.Instructor.Commands.CreateInstructor;

public sealed record CreateInstructorCommand(
    Guid Id,
    string Name,
    string Email,
    string Password,
    string PhoneNumber,
    string Role)
    : IRequest<Result<InstructorDto>>;
